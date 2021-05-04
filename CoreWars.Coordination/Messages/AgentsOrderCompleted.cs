using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Coordination.Messages
{
    public sealed class AgentsOrderCompleted
    {
        public AgentsOrderCompleted(IEnumerable<IAgentActorRef> agents)
        {
            Agents = agents.ToList();
            
        }
        public IReadOnlyList<IAgentActorRef> Agents { get; }
    }
}