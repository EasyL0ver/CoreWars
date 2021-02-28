using System;

namespace CoreWars.Competition
{
    public class CompetitionAgentMessage
    {
        public CompetitionAgentMessage(Guid agentId)
        {
            AgentId = agentId;
        }

        public Guid AgentId { get; }
        
    }
}