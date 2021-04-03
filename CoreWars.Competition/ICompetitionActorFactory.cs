using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Competition
{
    public interface ICompetitionActorFactory
    {
        IActorRef Build(IEnumerable<IActorRef> competitionParticipants, IActorContext context);
    }
}