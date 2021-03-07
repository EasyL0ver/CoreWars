using Autofac;
using CoreWars.Common;
using CoreWars.Competition.Modularity;

namespace PrisonerDilemma
{
    public class DilemmaModule : CompetitionModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
        }

        protected override void ConfigureCompetitionInfo(CompetitionInfo competitionInfo)
        {
            competitionInfo.Name = "prisoner-dilemma";
            competitionInfo.PlayerCount = Range<int>.Between(2,2);
            
            competitionInfo
                .RegisterApiAction<Messages.PresentDilemmaMessage, PrisonerAction>("present-dilemma-action");
        }
    }
}