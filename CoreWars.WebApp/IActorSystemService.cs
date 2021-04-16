using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Scripting;

namespace CoreWars.WebApp
{
    public interface IActorSystemService
    {
        ActorSystem ActorSystem { get; }
    }

    public interface IGameService
    {
        IReadOnlyList<Competition.ICompetition> AvailableCompetitions { get; }

        void AddCompetitor(Props competitorProps, Competition.ICompetition competition);
        void AddCompetition(Competition.ICompetition competition);
    }

}