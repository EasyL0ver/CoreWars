using System;

namespace CoreWars.Common
{
    public interface ILobbyConfig 
    {
        public string Name { get; }
        public Range<int> PlayerCount { get; }
        int MaxInstancesCount { get; }
        TimeSpan Timeout { get; }
    }
}