using CoreWars.Common;

namespace CoreWars.Competition
{
    public interface ICompetition
    {
        ICompetitionInfo Info { get; }
        ICompetitionActorPropsFactory Factory { get; }
    }

    public class Competition : ICompetition
    {
        public Competition(ICompetitionInfo configuration, ICompetitionActorPropsFactory factory)
        {
            Info = configuration;
            Factory = factory;
        }

        public ICompetitionInfo Info { get; }
        public ICompetitionActorPropsFactory Factory { get; }
    }
}