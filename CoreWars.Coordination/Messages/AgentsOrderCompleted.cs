using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Coordination.Messages
{
    public sealed class AgentsOrderCompleted
    {
        public AgentsOrderCompleted(IEnumerable<IActorRef> agents)
        {
            Agents = agents.ToList();
        }

        public IReadOnlyList<IActorRef> Agents { get; }
    }
}