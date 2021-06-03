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

        private readonly HashSet<IActorRef> _statusSubscriptions;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private Props _playerAgentActorFactory;
        private int _methodCallsFailureCount = 0;

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

            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<CompetitionResult>(OnGameConcluded);
            Receive<GameLog>(OnGameLogReceived);
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(OnStatsUpdated);
            Receive<Subscribe>(msg =>
            {
                Context.Watch(Sender);
                _statusSubscriptions.Add(Sender);
            });
            Receive<CompetitorFactoryUpdated>(OnUpdated);
            Receive<Terminated>(msg => { _statusSubscriptions.Remove(msg.ActorRef); });
        }

        private void OnUpdated(CompetitorFactoryUpdated obj)
        {
            _playerAgentActorFactory = obj.NewFactory;
            _methodCallsFailureCount = 0;
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
            var msg = new Data.Entities.Messages.ScriptCompetitionResult(_scriptInfo.Id, obj);
            _resultRepository.Tell(msg);
        }

        public static Props Props(Props factory, IActorRef playerLobby, IUser creator, IScriptInfo info, IActorRef resultRepository)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby, creator, info, resultRepository));
        }

        protected override void PreStart()
        {
            base.PreStart();

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero
                , TimeSpan.FromMilliseconds(RejoinLobbyTimeMilliseconds)
                , _playerLobby
                , RequestLobbyJoin.Instance
                , Self);
        }

        private void OnRequestCreateAgentReceived(RequestCreateAgent obj)
        {
            _logger.Debug($"Spawning new agent");

            var agentActorRef = Context.ActorOf(_playerAgentActorFactory);
            var credentialsWrapper = new GeneratedAgent(agentActorRef, _scriptInfo.Id);

            Context.Watch(agentActorRef);
            Sender.Tell(credentialsWrapper);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case AgentFailureException:
                        case ActorInitializationException:
                            throw new CompetitorFaultedException(
                                _scriptInfo.Id
                                , Diagnostics.FormatCompetitorFaultedMessage()
                                , ex);
                        case AgentMethodInvocationException:
                            _methodCallsFailureCount += 1;

                            if (_methodCallsFailureCount > MaxMethodCallsFailures)
                                throw new CompetitorFaultedException(
                                    _scriptInfo.Id
                                    , Diagnostics.FormatCompetitorInvalidMethodCallsExceeded(MaxMethodCallsFailures)
                                    , ex);
                            
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
}