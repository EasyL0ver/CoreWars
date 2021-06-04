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
        private AgentFailure _failureInfo;

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public Competitor(
            Props playerAgentActorFactory
            , IActorRef playerLobby
            , IUser creator
            , IScriptInfo scriptInfo
            , IActorRef resultRepository)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _scriptInfo = scriptInfo;
            _resultRepository = resultRepository;
            _statusSubscriptions = new HashSet<IActorRef>();
            _methodCallsFailureCounter = new Counter(MaxMethodCallsFailures);
            _state = scriptInfo.Faulted ? CompetitorState.Faulted : CompetitorState.Active;


            if (_scriptInfo.Faulted)
                Faulted();
            else
                Active();
        }

        private void Active()
        {
            ReactingToStatusMessages();

            Receive<RequestCreateAgent>(msg =>
            {
                _logger.Debug($"Spawning new agent");

                var agentActorRef = Context.ActorOf(_playerAgentActorFactory);
                var credentialsWrapper = new GeneratedAgent(agentActorRef, _scriptInfo.Id);

                Context.Watch(agentActorRef);
                Sender.Tell(credentialsWrapper);
            });

            Receive<AgentFailure>(msg =>
            {
                _failureInfo = msg;
                
                DisconnectFromLobby();
                UpdateState(CompetitorState.Faulted);
                Become(Faulted);

                throw new CompetitorFaultedException(
                    Self
                    , "agent failure"
                    , _failureInfo.Exception
                    , _scriptInfo.Id);
            });
            
            Receive<CompetitorFactoryUpdated>(msg =>
            {
                _playerAgentActorFactory = msg.NewFactory;
                _methodCallsFailureCounter.Reset();
                UpdateState(CompetitorState.Inconclusive);
            });
            
            Receive<CompetitionResult>(OnGameConcluded);
        }

        private void Faulted()
        {
            ReactingToStatusMessages();
            
            Receive<CompetitorFactoryUpdated>(msg =>
            {
                UpdateState(CompetitorState.Inconclusive);
                ConnectToLobby();
            });

            Receive<RequestCreateAgent>(msg =>
            {
                var ex = new CompetitorFaultedException(
                    Self
                    , "Unable to create agent- competitor is faulted critically"
                    , _failureInfo.Exception
                    , _scriptInfo.Id);
                
                Sender.Tell(ex);
            });
        }

        private void ReactingToStatusMessages()
        {
            Receive<GameLog>(OnGameLogReceived);
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(OnStatsUpdated);
            Receive<Subscribe>(msg =>
            {
                Context.Watch(Sender);
                _statusSubscriptions.Add(Sender);
                
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
            _logger.Info(obj.Message);
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
            _statusSubscriptions.ForEach(sub => sub.Tell(_state));
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
            IActorRef resultRepository)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby, creator, info, resultRepository));
        }

        protected override void PreStart()
        {
            base.PreStart();
            
            if(!_scriptInfo.Faulted)
                ConnectToLobby();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new AllForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case AgentFailureException:
                        case ActorInitializationException:
                            Self.Tell(new AgentFailure(ex));
                            return Directive.Stop;
                        case AgentMethodInvocationException:
                            _methodCallsFailureCounter.Increment();

                            if (!_methodCallsFailureCounter.Exceeded)
                                return Directive.Resume;
                            
                            Self.Tell(new AgentFailure(ex));
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
}