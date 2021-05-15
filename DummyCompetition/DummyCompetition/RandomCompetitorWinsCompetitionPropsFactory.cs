using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;

namespace DummyCompetition
{
    public class RandomCompetitorWinsCompetitionPropsFactory : ICompetitionActorPropsFactory
    {
        public Props Build(IEnumerable<IAgentActorRef> competitionParticipants)
        {
            return Props.Create(() => new BiggestNumberWinsCompetition(competitionParticipants));
        }
    }
}