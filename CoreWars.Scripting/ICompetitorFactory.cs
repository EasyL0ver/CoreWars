using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Data.Entities;

namespace CoreWars.Scripting
{
    public interface ICompetitorFactory
    {
        IReadOnlyList<string> SupportedCompetitionNames { get; }
        
        Props Build(GameScript script);
    }
}