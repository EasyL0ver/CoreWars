using Akka.Actor;

namespace CoreWars.Scripting
{
    public interface IClassProxyScriptCompetitorPropsFactory
    {
        Props Build(string script);
    }
}