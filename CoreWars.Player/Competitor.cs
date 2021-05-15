using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class Competitor : ReceiveActor
    {
        private const int RejoinLobbyTimeMilliseconds = 5000;

        private readonly Props _playerAgentActorFactory;
        private readonly IActorRef _playerLobby;
        private readonly IUser _creator;
        private readonly IScriptInfo _scriptInfo;
        private readonly WinRateCounter _winRateCounter;

        private readonly HashSet<IActorRef> _statusSubscriptions;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public Competitor(
            Props playerAgentActorFactory
            , IActorRef playerLobby
            , IUser creator
            , IScriptInfo scriptInfo
            , WinRateCounter winRateCounter)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _scriptInfo = scriptInfo;
            _winRateCounter = winRateCounter;
            _statusSubscriptions = new HashSet<IActorRef>();

            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<CompetitionResult>(OnGameConcluded);
            Receive<Subscribe>(msg =>
            {
                Context.Watch(Sender);
                _statusSubscriptions.Add(Sender);
            });
            Receive<Terminated>(msg => { _statusSubscriptions.Remove(msg.ActorRef); });
        }

        private void OnGameConcluded(CompetitionResult obj)
        {
            _winRateCounter.GamesPlayed += 1;

            if (obj == CompetitionResult.Winner)
                _winRateCounter.Wins += 1;

            UpdateStatusListeners();
        }

        public static Props Props(Props factory, IActorRef playerLobby, IUser creator, IScriptInfo info,
            WinRateCounter counter)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby, creator, info, counter));
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
            var credentialsWrapper = new AgentActorRef(agentActorRef, _creator, _scriptInfo);

            Context.Watch(agentActorRef);
            Sender.Tell(credentialsWrapper);
        }

        private void UpdateStatusListeners()
        {
            var status = new Common.CompetitorStatus(CompetitorState.Active, _winRateCounter.GamesPlayed,
                _winRateCounter.Wins);

            _statusSubscriptions.ForEach(sub => sub.Tell(status));
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case AgentFailureException:
                            return Directive.Escalate;
                        case AgentMethodInvocationException:
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
}