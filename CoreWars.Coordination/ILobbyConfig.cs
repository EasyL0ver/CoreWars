using System;
using CoreWars.Common;

namespace CoreWars.Coordination
{
    public interface ILobbyConfig : ICompetition
    {
        int MaxInstancesCount { get; }
        TimeSpan CreateCompetitorAgentTimeout { get; }
    }
}