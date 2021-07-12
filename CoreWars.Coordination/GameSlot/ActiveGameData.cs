using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Coordination.GameSlot
{
    public class ActiveGameData : ICompetitionSlotFSMData
    {
        public ActiveGameData(IActorRef game, IReadOnlyList<GeneratedAgent> competitors)
        {
            Game = game;
            Competitors = competitors;
        }

        public IActorRef Game { get; }
        public IReadOnlyList<GeneratedAgent> Competitors { get; }

        public override string ToString()
        {
            return $"Game with {Competitors.Count} competitors";
        }
    }
}