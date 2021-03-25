using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Coordination
{
    public class PlayerSet<IActorRef> : HashSet<Akka.Actor.IActorRef> , ISelectableSet<Akka.Actor.IActorRef>
    {
        private readonly ICollectionSelectionStrategy<Akka.Actor.IActorRef> _playerSelectionStrategy;

        public PlayerSet(
            ICollectionSelectionStrategy<Akka.Actor.IActorRef> playerSelectionStrategy)
        {
            _playerSelectionStrategy = playerSelectionStrategy;
        }

        public IEnumerable<Akka.Actor.IActorRef> Select(Range<int> amount)
        {
            return _playerSelectionStrategy.Select(this, amount);
        }
    }
}