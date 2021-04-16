using Akka.Actor;

namespace CoreWars.Scripting
{
    public interface ICompetitorFactory
    {
        Props Build(string script);
    }
}