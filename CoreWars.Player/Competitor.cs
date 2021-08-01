using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Player.Exceptions;
using CoreWars.Player.Messages;
using CoreWars.Scripting;

namespace CoreWars.Player
{
    public class Competitor : ReceiveActor
    {
        private const int RejoinLobbyTimeMilliseconds = 5000;
        private const int MaxMethodCallsFailures = 3;

        private readonly IActorRef _playerLobby;
        private readonly IUser _creator;
        private readonly IActorRef _resultRepository;
        private readonly Counter _methodCallsFailureCounter;

        private readonly HashSet<IActorRef> _statusSubscriptions;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly ICompetitorFactory _playerAgentActorFactory;

        private IScript _script;
        private CompetitorState _state;
        private ICancelable _connectToLobbyCancellable;

        //todo something better!
        private AgentFailureState _failureStateInfo;

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public Competitor(
            ICompetitorFactory playerAgentActorFactory
            , IActorRef playerLobby
            , IUser creator
            , IScript script
            , IActorRef resultRepository
            , CompetitorState initialState)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _script = script;
            _resultRepository = resultRepository;
            _statusSubscriptions = new HashSet<IActorRef>();
            _methodCallsFailureCounter = new Counter(MaxMethodCallsFailures);
            _state = initialState;

            if (_script.Exception != null)
            {
                _failureStateInfo = new AgentFailureState(new DeserializedException(_script.Exception));
                Faulted();
            }
            else
                Active();
        }

        private void Active()
        {
            ReactingToStatusMessages();

            Receive<RequestCreateAgent>(msg =>
            {
                _logger.Debug($"Spawning new agent");

                var props = _playerAgentActorFactory.Build(_script);
                var agent = Context.ActorOf(props);
                var message = new GeneratedAgent(agent, _script.Id);

                Sender.Tell(message);
            });

            Receive<AgentFailureState>(msg =>
            {
                UpdateFailure(msg);
                DisconnectFromLobby();
                UpdateState(CompetitorState.Faulted);
                Become(Faulted);

                throw new CompetitorFaultedException(
                    Self
                    , "agent failure"
                    , _failureStateInfo.Exception
                    , _script.Id);
            });

            Receive<CompetitorScriptUpdated>(msg =>
            {
                _script = msg.NewScript;
                _methodCallsFailureCounter.Reset();
                UpdateFailure(AgentFailureState.Ok());
                UpdateState(CompetitorState.Inconclusive);
            });

            Receive<CompetitionResult>(msg =>
            {
                UpdateState(CompetitorState.Active);
                var result = new Data.Entities.Messages.ScriptCompetitionResult(_script.Id, msg);
                _resultRepository.Tell(result);

                Sender.Tell(PoisonPill.Instance);
            });

            ReceiveAny(msg =>
                throw new InvalidOperationException($"Unknown message type received. Received object: {msg}"));
        }

        private void Faulted()
        {
            ReactingToStatusMessages();

            Receive<CompetitorScriptUpdated>(msg =>
            {
                _script = msg.NewScript;
                _methodCallsFailureCounter.Reset();

                UpdateFailure(AgentFailureState.Ok());
                UpdateState(CompetitorState.Inconclusive);
                ConnectToLobby();

                Become(Active);
            });

            Receive<RequestCreateAgent>(msg =>
            {
                var ex = new CompetitorFaultedException(
                    Self
                    , "Unable to create agent- competitor is faulted critically"
                    , _failureStateInfo.Exception
                    , _script.Id);

                Sender.Tell(ex);
            });

            Receive<CompetitionResult>(msg =>
            {
                //just in case 
                Sender.Tell(PoisonPill.Instance);
            });

            ReceiveAny(msg =>
                throw new InvalidOperationException($"Unknown message type received. Received object: {msg}"));
        }

        private void ReactingToStatusMessages()
        {
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(OnStatsUpdated);
            Receive<Subscribe>(msg =>
            {
                Context.Watch(Sender);
                _statusSubscriptions.Add(Sender);

                if (_failureStateInfo != null)
                    Sender.Tell(_failureStateInfo);

                Sender.Tell(_state);
                _resultRepository.Tell(
                    new Data.Entities.Messages.GetAllForCompetitor(_script.Id)
                    , Sender);
            });
            Receive<Terminated>(msg => { _statusSubscriptions.Remove(msg.ActorRef); });
        }

        private void OnStatsUpdated(Data.Entities.Messages.ScriptStatisticsUpdated obj)
        {
            _statusSubscriptions.ForEach(sub => sub.Tell(obj));
        }


        private void UpdateState(CompetitorState newState)
        {
            _state = newState;
            BroadcastToSubscribers(_state);
        }

        private void UpdateFailure(AgentFailureState newState)
        {
            _failureStateInfo = newState;
            BroadcastToSubscribers(_failureStateInfo);
        }

        private void BroadcastToSubscribers(object payload)
        {
            _statusSubscriptions.ForEach(sub => sub.Tell(payload));
        }

        private void ConnectToLobby()
        {
            _connectToLobbyCancellable?.Cancel();

            _connectToLobbyCancellable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.Zero
                , TimeSpan.FromMilliseconds(RejoinLobbyTimeMilliseconds)
                , _playerLobby
                , RequestLobbyJoin.Instance
                , Self);
        }

        private void DisconnectFromLobby()
        {
            _connectToLobbyCancellable?.Cancel();
            _playerLobby.Tell(RequestLobbyQuit.Instance);
        }

        public static Props Props(ICompetitorFactory factory, IActorRef playerLobby, IUser creator, IScript script,
            IActorRef resultRepository, CompetitorState initialState)
        {
            return Akka.Actor.Props.Create(() =>
                new Competitor(factory, playerLobby, creator, script, resultRepository, initialState));
        }

        protected override void PreStart()
        {
            base.PreStart();

            if (_script.Exception == null)
                ConnectToLobby();
        }

        protected override void PostStop()
        {
            base.PostStop();
            DisconnectFromLobby();
            _logger.Info("Competitor {0} with id: {1} stops", _script.Name, _script.Id);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            var decider = new LocalOnlyDecider(ex =>
            {
                switch (ex)
                {
                    case AgentFailureException:
                    case ActorInitializationException:
                        _logger.Info("Agent {0} with id {1} failed with exception {2}", _script.Name, _script.Id, ex);
                        Self.Tell(new AgentFailureState(ex));
                        return Directive.Stop;
                    case AgentMethodInvocationException:
                        _methodCallsFailureCounter.Increment();

                        if (!_methodCallsFailureCounter.Exceeded)
                            return Directive.Resume;

                        Self.Tell(new AgentFailureState(ex));
                        return Directive.Stop;
                    default:
                        _logger.Error("Unhandled exception thrown thrown within game agent: {0}", ex);
                        return Directive.Escalate;
                }
            });

            return new AllForOneStrategy(-1, -1, decider, false);
        }
    }
}