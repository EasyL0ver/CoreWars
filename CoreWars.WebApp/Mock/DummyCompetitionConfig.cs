using System;
using CoreWars.Common;
using CoreWars.Coordination;

namespace CoreWars.App.Mock
{
    public class DummyCompetitionConfig : ILobbyConfig
    {
        public string Name { get; } = "dummy competition";
        public Range<int> PlayerCount { get; } = Range<int>.Between(2,5);
        public int MaxInstancesCount { get; } = 1;
        public TimeSpan CreateCompetitorAgentTimeout { get; } = TimeSpan.FromSeconds(5);
    }
}