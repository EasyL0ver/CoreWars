using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor : ReceiveActor
    {
        protected CompetitionActor(
            IEnumerable<IActorRef> competitorActors)
        {
            Competitors = competitorActors.ToList();
            
            Receive<Messages.RunCompetitionMessage>(
                x => RunCompetition());
        }

        protected void AnnounceResult(
            CompetitionResultMessage resultMessage)
        {
            Context.Parent.Tell(resultMessage);
            Competitors.ForEach(c => c.Tell(resultMessage));
        }
        
        protected IReadOnlyList<IActorRef> Competitors { get; }
        
        protected abstract void RunCompetition();
    }
}