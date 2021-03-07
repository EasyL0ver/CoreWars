using System.Collections.Generic;
using CoreWars.Competition;

namespace PrisonerDilemma
{
    public interface IDilemmaPlayer : ICompetitionParticipant
    {
        int Score { get; set; }
        List<PrisonerAction> ActionLog { get; }
    }
}