using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.App.Mock
{
    public class RandomCompetitorWinsCompetitionPropsFactory : ICompetitionActorPropsFactory
    {
        public Props Build(IEnumerable<IActorRef> competitionParticipants)
        {
            return Props.Create(() => new RandomCompetitorWinsCompetition(competitionParticipants));
        }
    }
}