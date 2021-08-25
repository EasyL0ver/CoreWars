using Akka.Actor;

namespace CoreWars.Game.Messages
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