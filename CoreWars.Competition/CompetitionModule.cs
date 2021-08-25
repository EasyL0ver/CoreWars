using Autofac;
using CoreWars.Common;

namespace CoreWars.Competition
{
    public abstract class CompetitionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var competitionConfiguration = new CompetitionInfo();
            
            ConfigureCompetitionInfo(competitionConfiguration);

            var competition = new CompetitionRegistration(competitionConfiguration, ConfigureFactory());

            builder
                .RegisterInstance(competition)
                .As<ICompetitionRegistration>()
                .SingleInstance();
        }

        protected abstract ICompetitionActorPropsFactory ConfigureFactory();
        protected abstract void ConfigureCompetitionInfo(CompetitionInfo competitionInfo);
    }
}