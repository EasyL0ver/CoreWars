using Autofac;
using CoreWars.Common;

namespace CoreWars.Competition.Modularity
{
    public abstract class CompetitionModule : Module
    {
        public abstract Range<int> PlayersAllowedCount { get; }

        public abstract void RegisterApi();
    }
}