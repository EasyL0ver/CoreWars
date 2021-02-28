using System.Collections.Generic;
using BotArena;

namespace PrisonerDilemma
{
    public interface IDilemmaPlayer : ICompetitionAgent
    {
        int Score { get; set; }
        List<PrisonerAction> ActionLog { get; }
        PrisonerAction CurrentAction { get; set; }
        void ClearAction();
    }
}