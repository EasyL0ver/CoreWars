using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Game.FSMData
{
    public class ActiveGameData : ICompetitionSlotFSMData
    {
        public ActiveGameData(IActorRef game, IReadOnlyList<IActorPlayer> competitors)
        {
            Game = game;
            Competitors = competitors;
        }

        public IActorRef Game { get; }
        public IReadOnlyList<IActorPlayer> Competitors { get; }

        public override string ToString()
        {
            return $"Game with {Competitors.Count} competitors";
        }
    }
}