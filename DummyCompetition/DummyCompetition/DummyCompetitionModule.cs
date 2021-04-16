using Autofac;
using CoreWars.Common;
using CoreWars.Competition;

namespace DummyCompetition
{
    public class DummyCompetitionModule : CompetitionModule
    {
        protected override ICompetitionActorPropsFactory ConfigureFactory()
        {
            return new RandomCompetitorWinsCompetitionPropsFactory();
        }

        protected override void ConfigureCompetitionInfo(CompetitionInfo competitionInfo)
        {
            competitionInfo.Name = "dummy-competition";
            competitionInfo.PlayerCount = Range<int>.Between(2,5);
        }
    }
}