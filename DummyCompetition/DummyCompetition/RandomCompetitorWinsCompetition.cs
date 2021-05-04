using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace DummyCompetition
{
    public class RandomCompetitorWinsCompetition : CompetitionActor
    {
        private Dictionary<IActorRef, CompetitionResult> _result;
        public RandomCompetitorWinsCompetition(IEnumerable<IAgentActorRef> competitorActors) : base(competitorActors)
        {
        }

        protected override void RunCompetition()
        {
            var random = new Random();
            int index = random.Next(Competitors.Count);
            var winner = Competitors[index];

            _result = Competitors.ToDictionary(x => x,
                y => y.Equals(winner) ? CompetitionResult.Winner : CompetitionResult.Loser);
            
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Conclude();
        }

        protected override CompetitionResult GetResult(IActorRef playerActor)
        {
            return _result[playerActor];
        }
    }
}