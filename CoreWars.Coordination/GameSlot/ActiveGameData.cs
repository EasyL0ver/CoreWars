using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Coordination.GameSlot
{
    public class ActiveGameData : ICompetitionSlotFSMData
    {
        public ActiveGameData(IActorRef game, IReadOnlyList<IActorRef> competitors)
        {
            Game = game;
            Competitors = competitors;
        }

        public IActorRef Game { get; }
        public IReadOnlyList<IActorRef> Competitors { get; }

        public override string ToString()
        {
            return $"Game with {Competitors.Count} competitors";
        }
    }
}