using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreWars.Common
{
    public static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var element in source)
                action.Invoke(element);
        }

        public static string FormatString(this IEnumerable<string> source, string delimiter)
        {
            return string.Join(delimiter, source);
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