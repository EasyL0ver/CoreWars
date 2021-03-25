using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Coordination.Messages
{
    public sealed class LobbyPlayerTerminated : MessageWithActorRef
    {
        public LobbyPlayerTerminated(IActorRef actorRef) : base(actorRef)
        {
        }
    }
}