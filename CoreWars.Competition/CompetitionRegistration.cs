using CoreWars.Common;

namespace CoreWars.Competition
{
    public class CompetitionRegistration : ICompetitionRegistration
    {
        public CompetitionRegistration(ICompetitionInfo configuration, ICompetitionActorPropsFactory factory)
        {
            Info = configuration;
            Factory = factory;
        }

        public ICompetitionInfo Info { get; }
        public ICompetitionActorPropsFactory Factory { get; }
    }
}