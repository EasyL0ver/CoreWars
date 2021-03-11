using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Competition
{
    public abstract class CompetitionActor<TContext> : ReceiveActor
        where TContext : class
    {
        protected CompetitionActor()
        {
            Receive<Messages.RunCompetitionMessage>(
                x => RunCompetition());
        }
        
        protected abstract void RunCompetition();
    }
}