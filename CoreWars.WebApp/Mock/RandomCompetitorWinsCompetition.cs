using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.App.Mock
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
                y => y == winner ? CompetitionResult.Winner : CompetitionResult.Loser);
            
            AnnounceResult(new CompetitionResultMessage(result));
        }
        
        
    }
}