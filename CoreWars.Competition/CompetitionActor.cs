using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor : ReceiveActor
    {
        private readonly IReadOnlyList<IAgentActorRef> _competitors;
        
        protected CompetitionActor(
            IEnumerable<IAgentActorRef> competitorActors)
        {
            _competitors = competitorActors.ToList();
            
            Receive<Messages.RunCompetitionMessage>(
                x => RunCompetition());
        }

        private void AnnounceResult(
            CompetitionResultMessage resultMessage)
        {
            Context.Parent.Tell(resultMessage);
            Competitors.ForEach(c => c.Tell(resultMessage));
        }

        protected void Conclude()
        {
            var dict = _competitors.ToDictionary(x => x, GetResult);
            var message = new CompetitionResultMessage(dict);
            
            AnnounceResult(message);
        }

        protected IReadOnlyList<IActorRef> Competitors => _competitors;
        
        protected abstract void RunCompetition();
        protected abstract CompetitionResult GetResult(IActorRef playerActor);
    }
}