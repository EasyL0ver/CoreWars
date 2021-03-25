using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Coordination
{
    public interface ICollectionSelectionStrategy<T>
    {
        IEnumerable<T> Select(ICollection<T> pool, Range<int> amount);
    }
}