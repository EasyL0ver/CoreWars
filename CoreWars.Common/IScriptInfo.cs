using System;

namespace CoreWars.Common
{
    public interface IScriptInfo
    {
        Guid Id { get; }
        string Name { get; }
        string ScriptType { get; set; }
        string Exception { get;  }
    }
}