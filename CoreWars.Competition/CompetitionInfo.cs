using System;
using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public class CompetitionInfo : ICompetition
    {
            
        public string Name { get; set; }
        public Range<int> PlayerCount { get; set; }
        public Type ContextType { get; set; }


    }
}