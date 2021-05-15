using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common.TypedActorQuery.Query;
using CoreWars.Competition;

namespace DummyCompetition
{
    public class BiggestNumberWinsCompetition : CompetitionActor
    {
        private Dictionary<IActorRef, CompetitionResult> _result;
        private IActorRef _winner;
        
        public BiggestNumberWinsCompetition(IEnumerable<IAgentActorRef> competitorActors) : base(competitorActors)
        {
            Receive<TypedQueryResult<int>>(OnNumberPicked);
        }

        private void OnNumberPicked(TypedQueryResult<int> obj)
        {
            _winner = obj.Result
                .OrderByDescending(x => x.Value)
                .First()
                .Key;
            
            Conclude();
        }

        protected override void RunCompetition()
        {
            Competitors
                .QueryFor<int>(
                    new RunMethodMessage("pick_integer")
                    , Context
                    , TimeSpan.FromSeconds(5));
        }

        protected override CompetitionResult GetResult(IActorRef playerActor)
        {
            return _winner.Equals(playerActor) ? CompetitionResult.Winner : CompetitionResult.Loser;
        }
    }
}