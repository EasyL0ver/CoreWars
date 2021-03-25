using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Competition
{
    public interface ICompetitionActorPropsFactory
    {
        Props Build(IEnumerable<IActorRef> competitionParticipants);
    }
}