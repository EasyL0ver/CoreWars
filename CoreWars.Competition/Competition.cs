using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions;

namespace CoreWars.Competition
{
    public abstract class Competition : ReceiveActor
    {
        private readonly IReadOnlyList<GeneratedAgent> _competitors;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        protected Competition(
            IEnumerable<GeneratedAgent> competitorActors)
        {
            _competitors = competitorActors.ToList();
            
            Receive<RunCompetitionMessage>(
                x => RunCompetition());
        }

        private void AnnounceResult()
        {
            _competitors.ForEach(competitor =>
            {
                var result = GetResult(competitor.Reference);
                competitor.Reference.Tell(result);
            });
            
            Self.Tell(PoisonPill.Instance);
        }

        protected void Conclude()
        {
            AnnounceResult();
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
                            _logger.Warning("One of game agents is unresponsive - aborting with exception: {0}", ex);
                            
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