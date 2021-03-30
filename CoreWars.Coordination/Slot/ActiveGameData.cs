using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Coordination
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
    }
}