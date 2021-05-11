using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
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

        private readonly HashSet<IActorRef> _statusSubscriptions;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public Competitor(
            Props playerAgentActorFactory
            , IActorRef playerLobby
            , IUser creator
            , IScriptInfo scriptInfo)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _scriptInfo = scriptInfo;
            _statusSubscriptions = new HashSet<IActorRef>();

            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<Subscribe>(msg =>
            {
                _statusSubscriptions.Add(Sender);
            });
        }

        public static Props Props(Props factory, IActorRef playerLobby, IUser creator, IScriptInfo info)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby, creator, info));
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
            
            UpdateStatusListeners();
        }

        private void UpdateStatusListeners()
        {
            var status = new CompetitorStatus(CompetitorState.Active, null);
            var message = new CompetitorStatusChanged(status, _scriptInfo.Id);
            
            _statusSubscriptions.ForEach(sub => sub.Tell(message));
        }
    }
}