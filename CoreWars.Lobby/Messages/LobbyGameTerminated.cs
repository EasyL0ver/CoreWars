using Akka.Actor;

namespace CoreWars.Coordination.Messages
{
    public sealed class LobbyGameTerminated
    {
        public IActorRef ActorRef { get; }

        public LobbyGameTerminated(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
    }
}