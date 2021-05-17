using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Common
{
    public interface ICompetitionActorPropsFactory
    {
        Props Build(IEnumerable<GeneratedAgent> competitionParticipants);
    }
}