using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.App.Mock
{
    public class RandomCompetitorWinsCompetitionFactory : ICompetitionActorFactory
    {
        public IActorRef Build(IEnumerable<IActorRef> competitionParticipants, IActorContext context)
        {
            return context.ActorOf(Props.Create(() => new RandomCompetitorWinsCompetition(competitionParticipants)));
        }
    }
}