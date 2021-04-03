using System;
using System.Collections.Generic;
using System.Linq;
using CoreWars.Common;

namespace CoreWars.Coordination.PlayerSet
{
    public class SelectMaxRandomCollectionSelectionStrategy<T> : ICollectionSelectionStrategy<T>
    {
        
        public IEnumerable<T> Select(ICollection<T> pool, Range<int> amount)
        {
            if (pool.Count < amount.minimum)
            {
                var notEnoughPlayersMessage =
                    $"Not enough players in pool to use this strategy, minimum players required: {amount.minimum}";
                throw new InvalidOperationException(notEnoughPlayersMessage);
            }

            var random = new Random();
            var elementsToTake = Math.Min(pool.Count, amount.maximum);

            return pool.OrderBy(x => random.Next()).Take(elementsToTake);

        }
    }
}