using System;

namespace BotArena
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