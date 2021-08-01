using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Coordination.Messages
{
    public sealed class RoomCreated
    {
        public RoomCreated(IReadOnlyList<IActorPlayer> actorPlayers)
        {
            ActorPlayers = actorPlayers;
        }

        public IReadOnlyList<IActorPlayer> ActorPlayers { get; }
    }
}