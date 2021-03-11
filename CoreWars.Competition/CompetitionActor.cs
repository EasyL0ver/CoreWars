using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

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
        
        protected IReadOnlyList<IActorRef> Competitors { get; }
        
        protected abstract void RunCompetition();
    }
}