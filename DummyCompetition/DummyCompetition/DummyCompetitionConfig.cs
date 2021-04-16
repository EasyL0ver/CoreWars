using System;
using CoreWars.Common;

namespace DummyCompetition
{
    public class DummyCompetitionConfig : ICompetitionInfo
    {
        public string Name { get; } = "dummy competition";
        public Range<int> PlayerCount { get; } = Range<int>.Between(2,5);
        public int MaxInstancesCount { get; } = 1;
        public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
    }
}