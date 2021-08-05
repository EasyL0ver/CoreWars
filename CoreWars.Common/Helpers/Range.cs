using System;

namespace CoreWars.Common
{
    public readonly struct Range<T> where T : IComparable<T>
    {
        public readonly T minimum;
        public readonly T maximum;

        private Range(T minimum, T maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public static Range<T> Between(T min, T max)
        {
            return new Range<T>(min, max);
        }

        public static Range<T> Exactly(T value)
        {
            return new Range<T>(value, value);
        }

        public bool IsWithin(T value)
        {
            return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0;
        }
        
    }
}