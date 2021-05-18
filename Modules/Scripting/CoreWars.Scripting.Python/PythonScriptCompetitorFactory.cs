using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Scripting.Python
{
    public class PythonScriptCompetitorFactory : ICompetitorFactory
    {
        public IReadOnlyList<string> SupportedCompetitionNames => new[] {"python"};

        public Props Build(IScript script)
        {
            var classProxy = new PythonInteroperabilityClassProxy(script.ScriptFiles[0]);
            return Props.Create(() => new ClassProxyScriptCompetitor(classProxy));
        }
    }
}