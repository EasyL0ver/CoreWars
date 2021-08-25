using Akka.Actor;

namespace CoreWars.Coordination.Messages
{
    public sealed class CompetitorTerminated
    {
        public IActorRef ActorRef { get; }

        public CompetitorTerminated(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
    }
}