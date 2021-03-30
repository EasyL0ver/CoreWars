using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Competition
{
    public interface ICompetitionActorPropsFactory
    {
        IActorRef Build(IEnumerable<IActorRef> competitionParticipants, IActorContext context);
    }
}