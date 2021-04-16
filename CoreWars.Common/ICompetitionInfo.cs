using System;

namespace CoreWars.Common
{
    public interface ICompetitionInfo 
    {
        public string Name { get; }
        public Range<int> PlayerCount { get; }
        int MaxInstancesCount { get; }
        TimeSpan Timeout { get; }
    }
}