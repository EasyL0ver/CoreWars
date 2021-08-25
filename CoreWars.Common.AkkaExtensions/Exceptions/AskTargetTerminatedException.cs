using System;
using Akka.Actor;

namespace CoreWars.Common.AkkaExtensions
{
    public class AskTargetTerminatedException : Exception
    {
        public AskTargetTerminatedException(IActorRef terminatedTarget, string message) : base(message)
        {
            TerminatedTarget = terminatedTarget;
        }

        public IActorRef TerminatedTarget { get; }
    }
}