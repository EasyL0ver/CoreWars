using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Player.Exceptions;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class Competitor : ReceiveActor
    {
        private const int RejoinLobbyTimeMilliseconds = 5000;
        private const int MaxMethodCallsFailures = 3;

        private readonly IActorRef _playerLobby;
        private readonly IUser _creator;
        private readonly IScriptInfo _scriptInfo;
        private readonly IActorRef _resultRepository;
        private readonly Counter _methodCallsFailureCounter;

        private readonly HashSet<IActorRef> _statusSubscriptions;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private Props _playerAgentActorFactory;
        private CompetitorState _state;
        private ICancelable _connectToLobbyCancellable;

        //todo something better!
        private AgentFailureState _failureStateInfo;

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public Competitor(
            Props playerAgentActorFactory
            , IActorRef playerLobby
            , IUser creator
            , IScriptInfo scriptInfo
            , IActorRef resultRepository
            , CompetitorState initialState)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _scriptInfo = scriptInfo;
            _resultRepository = resultRepository;
            _statusSubscriptions = new HashSet<IActorRef>();
            _methodCallsFailureCounter = new Counter(MaxMethodCallsFailures);
            _state = initialState;

            try
            {
                if (_scriptInfo.Exception != null)
                {
                    _failureStateInfo = new AgentFailureState(new DeserializedException(_scriptInfo.Exception));
                    Faulted();
                }
                else
                    Active();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Active()
        {
            ReactingToStatusMessages();

            Receive<RequestCreateAgent>(msg =>
            {
                _logger.Debug($"Spawning new agent");

                var agent = Context.ActorOf(_playerAgentActorFactory);
                var message = new GeneratedAgent(agent, _scriptInfo.Id);

                Context.Watch(agent);
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
                    , _scriptInfo.Id);
            });
            
            Receive<CompetitorFactoryUpdated>(msg =>
            {
                _playerAgentActorFactory = msg.NewFactory;
                _methodCallsFailureCounter.Reset();
                UpdateFailure(AgentFailureState.Ok());
                UpdateState(CompetitorState.Inconclusive);
            });
            
            Receive<CompetitionResult>(OnGameConcluded);
            ReceiveAny(msg => _logger.Error("Unknown messege received"));
        }

        private void Faulted()
        {
            ReactingToStatusMessages();
            
            Receive<CompetitorFactoryUpdated>(msg =>
            {
                _playerAgentActorFactory = msg.NewFactory;
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
                    , _scriptInfo.Id);
                
                Sender.Tell(ex);
            });
            
            ReceiveAny(msg => _logger.Error("Unknown messege received"));
        }

        private void ReactingToStatusMessages()
        {
            Receive<GameLog>(OnGameLogReceived);
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(OnStatsUpdated);
            Receive<Subscribe>(msg =>
            {
                Context.Watch(Sender);
                _statusSubscriptions.Add(Sender);
                
                if(_failureStateInfo != null)
                    Sender.Tell(_failureStateInfo);
                
                Sender.Tell(_state);
                _resultRepository.Tell(
                    new Data.Entities.Messages.GetAllForCompetitor(_scriptInfo.Id)
                    , Sender);
            });
            Receive<Terminated>(msg => { _statusSubscriptions.Remove(msg.ActorRef); });
        }

        private void OnStatsUpdated(Data.Entities.Messages.ScriptStatisticsUpdated obj)
        {
            _statusSubscriptions.ForEach(sub => sub.Tell(obj));
        }

        private void OnGameLogReceived(GameLog obj)
        {
            _logger.Debug(obj.Message);
        }

        private void OnGameConcluded(CompetitionResult obj)
        {
            UpdateState(CompetitorState.Active);
            var msg = new Data.Entities.Messages.ScriptCompetitionResult(_scriptInfo.Id, obj);
            _resultRepository.Tell(msg);
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

        public static Props Props(Props factory, IActorRef playerLobby, IUser creator, IScriptInfo info,
            IActorRef resultRepository, CompetitorState initialState)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby, creator, info, resultRepository, initialState));
        }

        protected override void PreStart()
        {
            base.PreStart();
            
            if(_scriptInfo.Exception == null)
                ConnectToLobby();
        }
        
        protected override void PostStop()
        {
            base.PostStop();
            DisconnectFromLobby();
            _logger.Info("Competitor {0} with id: {1} stops", _scriptInfo.Name, _scriptInfo.Id);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            var decider = new LocalOnlyDecider(ex =>
            {
                switch (ex)
                {
                    case AgentFailureException:
                    case ActorInitializationException:
                        _logger.Info("Agent {0} with id {1} failed with exception {2}", _scriptInfo.Name, _scriptInfo.Id, ex);
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
            
            return new AllForOneStrategy(-1,-1, decider, false);
        }
    }
}