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
            Receive<Messages.RequestContextMessage>(
                (msg) => Sender.Tell(GetGameContext(Sender)));

            Receive<Messages.RunCompetitionMessage>(
                x => RunCompetition());
        }

        protected virtual TContext GetGameContext(IActorRef sender)
        {
            return null as TContext;
        }
        
        protected abstract void RunCompetition();
    }
}