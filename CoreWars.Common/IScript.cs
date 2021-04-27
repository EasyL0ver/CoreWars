using System;

namespace CoreWars.Common
{
    public interface IScript
    {
        Guid Id { get; set; }
        string ScriptType { get; set; }
        string[] ScriptFiles { get; set; }
        string CompetitionName { get; }
    }
}