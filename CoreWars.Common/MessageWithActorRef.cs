using Akka.Actor;

namespace CoreWars.Common
{
    public abstract class MessageWithActorRef
    {
        protected MessageWithActorRef(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }

        public IActorRef ActorRef { get; }
    }
}