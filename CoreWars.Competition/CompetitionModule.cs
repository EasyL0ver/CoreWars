using System;
using System.IO;
using System.Net;
using System.Net.Mime;
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

            var competition = new Competition(competitionConfiguration, ConfigureFactory(), ScanForCodeTemplate());

            builder
                .RegisterInstance(competition)
                .As<ICompetition>()
                .SingleInstance();
        }

        protected abstract ICompetitionActorPropsFactory ConfigureFactory();
        protected abstract void ConfigureCompetitionInfo(CompetitionInfo competitionInfo);

        protected virtual string GetTemplateName()
        {
            return null;
        }

        private IPythonCodeTemplate ScanForCodeTemplate()
        {
            var templateName = GetTemplateName();
            if (templateName == null) return null;
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateName);
            var templateText = File.ReadAllText(templatePath);
            return new CodeTemplate(templateText);
        }
    }
}