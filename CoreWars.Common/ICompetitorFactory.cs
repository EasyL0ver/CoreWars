using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Common
{
    public interface ICompetitorFactory
    {
        IReadOnlyList<string> SupportedCompetitionNames { get; }
        
        Props Build(IScript script);
    }
}