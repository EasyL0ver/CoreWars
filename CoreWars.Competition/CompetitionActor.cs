using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common.TypedActorQuery.Ask;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor : ReceiveActor
    {
        private readonly IReadOnlyList<GeneratedAgent> _competitors;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
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
            _competitors.ForEach(competitor =>
            {
                var result = resultMessage.CompetitionResults[competitor.ScriptId];
                competitor.Reference.Tell(result);
            });
            
            Self.Tell(PoisonPill.Instance);
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
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                loggingEnabled: false,
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case TimeoutException _:
                        case AskTargetTerminatedException _:
                            _logger.Warning("One of game agents is unresponsive - aborting", ex);
                            
                            //kill all competitors
                            _competitors.ForEach(c => c.Reference.Tell(PoisonPill.Instance));
                            Self.Tell(PoisonPill.Instance);
                            
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
        
    }
}