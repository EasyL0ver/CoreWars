using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Coordination
{
    public interface ISelectableSet<T> : ISet<T>
    {
        public IEnumerable<T> Select(Range<int> amount);
    }
}