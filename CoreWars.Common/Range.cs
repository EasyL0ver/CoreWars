using System;

namespace CoreWars.Common
{
    public struct Range<T> where T : IComparable<T>
    {
        public T minimum;
        public T maximum;

        public Range(T minimum, T maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public static Range<T> Between(T min, T max)
        {
            return new Range<T>(min, max);
        }
        
    }
}