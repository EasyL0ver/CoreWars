using Akka.Actor;
using CoreWars.Common;
using System;

namespace CoreWars.Player.Messages
{
    public class GeneratedAgent : IActorPlayer
    {
        public GeneratedAgent(IActorRef actorRef, Guid scriptId)
        {
            ActorRef = actorRef;
            ScriptId = scriptId;
        }

        public IActorRef ActorRef { get; }
        public Guid ScriptId
        {
            get;
        }
}
}