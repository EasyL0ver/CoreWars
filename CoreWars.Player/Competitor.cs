using System;
using Akka.Actor;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class Competitor : ReceiveActor
    {
        private const int RejoinLobbyTimeMilliseconds = 5000;
        //todo private readonly IPlayerStats _playerStats;
        private readonly IPlayerAgentActorFactory _playerAgentActorFactory;
        private readonly IActorRef _playerLobby;
        //todo private readonly IPlayerActorCredentials _credentials;
        
        public Competitor(IPlayerAgentActorFactory playerAgentActorFactory, IActorRef playerLobby)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            _playerLobby = playerLobby;

            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<RequestPlayerCredentials>(OnRequestPlayerCredentialsReceived);
        }

        public static Props Props(IPlayerAgentActorFactory factory, IActorRef playerLobby)
        {
            return Akka.Actor.Props.Create(() => new Competitor(factory, playerLobby));
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
            var agentActorRef = _playerAgentActorFactory.Build(Context);
            Context.Watch(agentActorRef);
            Sender.Tell(new AgentCreated(agentActorRef));
        }

        private void OnRequestPlayerCredentialsReceived(RequestPlayerCredentials obj)
        {
            throw new System.NotImplementedException();
        }
    }
}