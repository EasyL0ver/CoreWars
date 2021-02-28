using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWars.Common
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var element in source)
                action.Invoke(element);
        }

        public static bool IsUniform<T>(this IEnumerable<T> source)
            where T : IEquatable<T>
        {
            var sourceList = source.ToList();

            if (sourceList.Count < 2)
                return true;

            return sourceList.All(x => x.Equals(sourceList[0]));
        }

    }
}