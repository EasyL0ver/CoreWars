namespace CoreWars.Common
{
    public interface IScript : IScriptInfo
    {
        string[] ScriptFiles { get; set; }
    }
}