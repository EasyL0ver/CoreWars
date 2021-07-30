using System;
using System.Collections.Generic;
using System.Linq;
using CoreWars.Common;
using CoreWars.Common.Exceptions;

namespace CoreWars.Coordination.PlayerSet
{
    public class SelectMaxRandomCollectionSelectionStrategy<T> : ICollectionSelectionStrategy<T>
    {
        
        public IEnumerable<T> Select(ICollection<T> pool, Range<int> amount)
        {
            if (pool.Count < amount.minimum)
            {
                throw new NotEnoughPlayersException(amount.minimum, pool.Count);
            }

            var random = new Random();
            var elementsToTake = Math.Min(pool.Count, amount.maximum);

            return pool.OrderBy(x => random.Next()).Take(elementsToTake);

        }
    }
}