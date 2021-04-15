using CoreWars.Common;

namespace CoreWars.Competition
{
    public interface ICompetition
    {
        ILobbyConfig Configuration { get; }
        ICompetitionActorPropsFactory Factory { get; }
    }

    public class Competition : ICompetition
    {
        public Competition(ILobbyConfig configuration, ICompetitionActorPropsFactory factory)
        {
            Configuration = configuration;
            Factory = factory;
        }

        public ILobbyConfig Configuration { get; }
        public ICompetitionActorPropsFactory Factory { get; }
    }
}