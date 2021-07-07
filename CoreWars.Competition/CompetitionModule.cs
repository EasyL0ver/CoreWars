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

            var competition = new Competition(competitionConfiguration, ConfigureFactory());

            builder
                .RegisterInstance(competition)
                .As<ICompetition>()
                .SingleInstance();
        }

        protected abstract ICompetitionActorPropsFactory ConfigureFactory();
        protected abstract void ConfigureCompetitionInfo(CompetitionInfo competitionInfo);
    }
}