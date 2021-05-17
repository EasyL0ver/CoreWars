using System;
using Akka.Actor;

namespace CoreWars.Common
{
    public class GeneratedAgent
    {
        public GeneratedAgent(IActorRef reference, Guid scriptId)
        {
            Reference = reference;
            ScriptId = scriptId;
        }

        public IActorRef Reference { get; }
        public Guid ScriptId
        {
            get;
        }
}
}