using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace PrisonerDilemma
{
    public class DilemmaModule : CompetitionModule
    {
        private class DilemmaFactory : ICompetitionActorPropsFactory
        {
            public Props Build(IEnumerable<IActorPlayer> competitionParticipants)
            {
                return Props.Create(() =>
                    new DilemmaCompetition(competitionParticipants, DilemmaConfiguration.Default()));
            }
        }
        
        protected override ICompetitionActorPropsFactory ConfigureFactory()
        {
            return new DilemmaFactory();
        }

        protected override void ConfigureCompetitionInfo(CompetitionInfo competitionInfo)
        {
            competitionInfo.Name = "prisoner-dilemma";
            competitionInfo.PlayerCount = Range<int>.Between(2,2);
            competitionInfo.MaxInstancesCount = 3;
        }
    }
}