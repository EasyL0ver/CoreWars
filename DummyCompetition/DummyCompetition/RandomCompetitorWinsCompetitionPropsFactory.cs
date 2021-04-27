using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace DummyCompetition
{
    public class RandomCompetitorWinsCompetitionPropsFactory : ICompetitionActorPropsFactory
    {
        public Props Build(IEnumerable<IActorRef> competitionParticipants)
        {
            return Props.Create(() => new RandomCompetitorWinsCompetition(competitionParticipants));
        }
    }
}