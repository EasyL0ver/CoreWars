namespace CoreWars.Common
{
    public class Counter
    {
        private readonly int _countTo;

        public Counter(int countTo)
        {
            _countTo = countTo;
        }
        
        public int Current { get; private set; }

        public void Increment()
        {
            Current += 1;
        }

        public void Reset()
        {
            Current = 0;
        }

        public bool Exceeded => Current >= _countTo;
    }
}