using Akka.Actor;

namespace CoreWars.Scripting.Python
{
    public class PythonCompetitorPropsFactory : IClassProxyScriptCompetitorPropsFactory
    {
        public Props Build(string script)
        {
            var classProxy = new PythonInteroperabilityClassProxy(script);
            return Props.Create<ClassProxyScriptCompetitor>(() => new ClassProxyScriptCompetitor(classProxy));
        }
    }
}