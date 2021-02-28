using System;
using System.Linq;

namespace PrisonerDilemma.SampleBot
{
    public class RevengeSampleActor : BaseContextSampleActor
    {
        protected override PrisonerAction Decide(DilemmaContext opponentContext)
        {
            if (!opponentContext.OpponentActionLog.Any())
                return PrisonerAction.Cooperate;
            
            return opponentContext.OpponentActionLog.Last() == PrisonerAction.Defect
                ? PrisonerAction.Defect
                : PrisonerAction.Cooperate;
        }

   
    }
}