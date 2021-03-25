using Akka.Actor;

namespace CoreWars.Coordination.Messages
{
    public class AddPlayer
    {
        public AddPlayer(IActorRef addedPlayerReference)
        {
            AddedPlayerReference = addedPlayerReference;
        }

        public IActorRef AddedPlayerReference { get; }
    }
}