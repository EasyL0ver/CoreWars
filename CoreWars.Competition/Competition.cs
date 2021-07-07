using CoreWars.Common;

namespace CoreWars.Competition
{
    public class Competition : ICompetition
    {
        public Competition(ICompetitionInfo configuration, ICompetitionActorPropsFactory factory, IPythonCodeTemplate codeTemplate)
        {
            Info = configuration;
            Factory = factory;
            CodeTemplate = codeTemplate;
        }

        public ICompetitionInfo Info { get; }
        public ICompetitionActorPropsFactory Factory { get; }
        public IPythonCodeTemplate CodeTemplate { get; }
    }
}