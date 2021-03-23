using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor : ReceiveActor
    {
        protected static void QueryFor<T>(
            IEnumerable<IActorRef> competitors
            , object message
            , TimeSpan timeout)
        {
            var queryActorProps = Props.Create<TypedQueryActor<T>>(
                competitors
                , message
                , QueryResultStrategy.ReportResultToParent<T>()
                , timeout);
            
            Context.ActorOf(queryActorProps);
        }
        
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