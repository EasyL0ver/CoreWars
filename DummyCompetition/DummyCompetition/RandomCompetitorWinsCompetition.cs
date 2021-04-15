using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using CoreWars.Competition;

namespace DummyCompetition
{
    public class RandomCompetitorWinsCompetition : CompetitionActor
    {
        public RandomCompetitorWinsCompetition(IEnumerable<IActorRef> competitorActors) : base(competitorActors)
        {
        }

        protected override void RunCompetition()
        {
            var random = new Random();
            int index = random.Next(Competitors.Count);
            var winner = Competitors[index];

            var result = Competitors.ToDictionary(x => x,
                y => y.Equals(winner) ? CompetitionResult.Winner : CompetitionResult.Loser);
            
            Thread.Sleep(TimeSpan.FromSeconds(5));
            AnnounceResult(new CompetitionResultMessage(result));
        }
        
        
    }
}