using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Coordination.Messages
{
    public sealed class LobbyGameTerminated : MessageWithActorRef
    {
        public LobbyGameTerminated(IActorRef actorRef) : base(actorRef)
        {
        }
    }
}