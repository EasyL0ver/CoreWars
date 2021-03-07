using System;
using System.Collections.Generic;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public interface ICompetition
    {
        public string Name { get; }
        public Range<int> PlayerCount { get; }
        public Type ContextType { get; } 
    }
}