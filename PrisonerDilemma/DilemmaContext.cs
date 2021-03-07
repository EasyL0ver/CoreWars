using System.Collections.Generic;
using CoreWars.Competition;

namespace PrisonerDilemma
{
    public class DilemmaContext 
    {
        public int Score { get; set; } 
        public int OpponentScore { get; set; }
        public List<PrisonerAction> OpponentActionLog { get; set; } 
        public IReadOnlyList<ICompetitionParticipant> Opponents { get; set; }
    }
}