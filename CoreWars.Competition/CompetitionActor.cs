using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor : ReceiveActor
    {
        private readonly IReadOnlyList<GeneratedAgent> _competitors;
        
        protected CompetitionActor(
            IEnumerable<GeneratedAgent> competitorActors)
        {
            _competitors = competitorActors.ToList();
            
            Receive<Messages.RunCompetitionMessage>(
                x => RunCompetition());
        }

        private void AnnounceResult(
            CompetitionResultMessage resultMessage)
        {
            Context.Parent.Tell(resultMessage);
            
            _competitors.ForEach(competitor =>
            {
                var result = resultMessage.CompetitionResults[competitor.ScriptId];
                competitor.Reference.Tell(result);
            });
        }

        protected void Conclude()
        {
            var dict = _competitors.ToDictionary(x => x.ScriptId,y => GetResult(y.Reference));
            var message = new CompetitionResultMessage(dict);
            
            AnnounceResult(message);
        }

        protected IReadOnlyList<IActorRef> Competitors => _competitors.Select(x => x.Reference).ToList();
        
        protected abstract void RunCompetition();
        protected abstract CompetitionResult GetResult(IActorRef playerActor);
    }
}