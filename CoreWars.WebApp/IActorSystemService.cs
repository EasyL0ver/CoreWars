using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;
using CoreWars.Scripting;

namespace CoreWars.WebApp
{
    public interface IActorSystemService
    {
        ActorSystem ActorSystem { get; }
    }

    public interface IGameService
    {
        IReadOnlyList<ICompetitionInfo> AvailableCompetitions { get; }

        void AddScript(GameScript gameScript);
    }

}