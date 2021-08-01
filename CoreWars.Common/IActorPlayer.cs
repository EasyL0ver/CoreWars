using Akka.Actor;
using System;

namespace CoreWars.Common
{
    public interface IActorPlayer
    {
        IActorRef ActorRef { get; }
        Guid ScriptId { get; }
    }
}