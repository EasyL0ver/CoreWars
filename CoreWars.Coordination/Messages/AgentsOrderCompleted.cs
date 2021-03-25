using System;
using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Coordination.Messages
{
    public sealed class AgentsOrderCompleted
    {
        public AgentsOrderCompleted(Guid requestId, IEnumerable<IActorRef> agents)
        {
            RequestId = requestId;
            Agents = agents;
        }

        public Guid RequestId { get; }
        public IEnumerable<IActorRef> Agents { get; }
    }
}