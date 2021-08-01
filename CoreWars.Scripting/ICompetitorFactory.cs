using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Scripting
{
    public interface ICompetitorFactory
    {
        IReadOnlyList<string> SupportedCompetitionNames { get; }
        
        Props Build(IScript script);
    }
}