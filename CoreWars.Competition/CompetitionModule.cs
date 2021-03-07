using Autofac;

namespace CoreWars.Competition
{
    public abstract class CompetitionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var competitionConfiguration = new CompetitionInfo();
            
            ConfigureCompetitionInfo(competitionConfiguration);

            builder
                .RegisterInstance(competitionConfiguration)
                .As<ICompetition>()
                .SingleInstance();
        }

        protected abstract void ConfigureCompetitionInfo(CompetitionInfo competitionInfo);
    }
}