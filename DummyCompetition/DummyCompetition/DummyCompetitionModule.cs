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

        protected override void ConfigureCompetitionInfo(LobbyConfig lobbyConfig)
        {
            lobbyConfig.Name = "dummy-competition";
            lobbyConfig.PlayerCount = Range<int>.Between(2,5);
        }
    }
}