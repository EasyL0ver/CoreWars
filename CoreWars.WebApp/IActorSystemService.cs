using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;

namespace CoreWars.WebApp
{
    public interface IActorSystemService
    {
        ActorSystem ActorSystem { get; }
    }

    public interface IGameService : IActorSystemService
    {
        IReadOnlyList<ICompetitionInfo> AvailableCompetitions { get; }
        
        IActorRef ResultsHandler { get; }
        IActorRef ScriptRepository { get; }
    }

}