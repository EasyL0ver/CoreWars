using System;
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
        
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        
        public Competitor(Props playerAgentActorFactory, IActorRef playerLobby, IUser creator, IScriptInfo scriptInfo)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;
            _creator = creator;
            _scriptInfo = scriptInfo;

            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<RequestPlayerCredentials>(OnRequestPlayerCredentialsReceived);
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
        }

        private void OnRequestPlayerCredentialsReceived(RequestPlayerCredentials obj)
        {
            throw new System.NotImplementedException();
        }
    }
}