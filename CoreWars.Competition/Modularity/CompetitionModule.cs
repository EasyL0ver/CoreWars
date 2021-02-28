using Autofac;
using BotArena.Common;

namespace BotArena.Modularity
{
    public abstract class CompetitionModule : Module
    {
        public abstract Range<int> PlayersAllowedCount { get; }

        public abstract void RegisterApi();
    }
}