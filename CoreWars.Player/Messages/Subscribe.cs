using Akka.Actor;

namespace CoreWars.Player.Messages
{
    public sealed class Subscribe
    {
        public static Subscribe Instance => new Subscribe();
    }

    public sealed class ListenerTerminated
    {
        public ListenerTerminated(IActorRef listenerRef)
        {
            ListenerRef = listenerRef;
        }

        public IActorRef ListenerRef { get; }
    }
}