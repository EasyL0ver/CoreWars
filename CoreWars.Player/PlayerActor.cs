using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class PlayerActor : ReceiveActor
    {
        //todo private readonly IPlayerStats _playerStats;
        private readonly IPlayerAgentPropsFactory _playerAgentPropsFactory;
        //todo private readonly IPlayerActorCredentials _credentials;
        
        public PlayerActor(IPlayerAgentPropsFactory playerAgentPropsFactory)
        {
            _playerAgentPropsFactory = playerAgentPropsFactory;
            
            Receive<RequestCreateAgent>(OnRequestCreateAgentReceived);
            Receive<RequestPlayerCredentials>(OnRequestPlayerCredentialsReceived);
        }

        private void OnRequestCreateAgentReceived(RequestCreateAgent obj)
        {
            var props = _playerAgentPropsFactory.Build();
            var agentActorRef = Context.ActorOf(props);
            
            Context.Watch(agentActorRef);
            Sender.Tell(new AgentCreated(agentActorRef));
        }

        private void OnRequestPlayerCredentialsReceived(RequestPlayerCredentials obj)
        {
            throw new System.NotImplementedException();
        }
    }
}