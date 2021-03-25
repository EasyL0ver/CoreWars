using Akka.Actor;

namespace CoreWars.Player.Messages
{
    public class AgentCreated
    {
        public AgentCreated(IActorRef agentReference)
        {
            AgentReference = agentReference;
        }

        public IActorRef AgentReference { get; }
    }
}