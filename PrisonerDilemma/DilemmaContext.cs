using System.Collections.Generic;
using BotArena;

namespace PrisonerDilemma
{
    public class DilemmaContext : ICompetitionContext
    {
        public int Score { get; set; } 
        public int OpponentScore { get; set; }
        public List<PrisonerAction> OpponentActionLog { get; set; } 
        public IReadOnlyList<ICompetitionAgentInfo> Opponents { get; set; }
    }
}