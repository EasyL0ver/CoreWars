using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;
using CoreWars.Scripting;

namespace CoreWars.WebApp
{
    public class AggregatedCompetitorFactory : ICompetitorFactory
    {
        private readonly List<ICompetitorFactory> _factories;
        public AggregatedCompetitorFactory(IEnumerable<ICompetitorFactory> factories)
        {
            _factories = factories.ToList();
        }

        public IReadOnlyList<string> SupportedCompetitionNames =>
            _factories.SelectMany(x => x.SupportedCompetitionNames).ToList();

        public Props Build(IScript script)
        {
            var innerFactory = _factories.Single(x => x.SupportedCompetitionNames.Contains(script.ScriptType));

            return innerFactory.Build(script);
        }
    }
}