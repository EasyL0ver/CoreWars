using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class PlayerActor : ReceiveActor
    {
        //todo private readonly IPlayerStats _playerStats;
        private readonly IPlayerAgentActorFactory _playerAgentActorFactory;
        //todo private readonly IPlayerActorCredentials _credentials;
        
        public PlayerActor(IPlayerAgentActorFactory playerAgentActorFactory)
        {
            _playerAgentActorFactory = playerAgentActorFactory;
            
            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<RequestPlayerCredentials>(OnRequestPlayerCredentialsReceived);
        }

        public static Props Props(IPlayerAgentActorFactory factory)
        {
            return Akka.Actor.Props.Create<PlayerActor>(() => new PlayerActor(factory));
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