using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Competition
{
    public interface ICompetitionContext
    {
        IReadOnlyList<ICompetitionAgentInfo> Opponents { get; }
    }
    public abstract class CompetitionActor<TContext> : ReceiveActor
        where TContext : ICompetitionContext
    {
        protected CompetitionActor()
        {
            Receive<Messages.RequestContextMessage>(
                (msg) =>
            {
                Sender.Tell(GetGameContext(Sender));
            });

            Receive<Messages.RunCompetitionMessage>(RunCompetition);
        }
        protected abstract IReadOnlyList<ICompetitionAgent> Players { get; }
        protected abstract void RunCompetition(Messages.RunCompetitionMessage message);
        protected abstract TContext GetGameContext(IActorRef playerRef);
    }
}