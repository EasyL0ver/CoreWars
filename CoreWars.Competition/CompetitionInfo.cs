using System;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public class CompetitionInfo : ICompetitionInfo
    {
        public string Name { get; set; }
        public Range<int> PlayerCount { get; set; }
        public int MaxInstancesCount { get; set; } = 10;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}