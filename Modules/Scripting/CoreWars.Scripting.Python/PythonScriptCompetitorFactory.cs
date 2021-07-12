using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using Microsoft.Scripting.Hosting;

namespace CoreWars.Scripting.Python
{
    public class PythonScriptCompetitorFactory : ICompetitorFactory
    {
        public IReadOnlyList<string> SupportedCompetitionNames => new[] {"python"};

        private readonly ScriptEngine _engine;

        public PythonScriptCompetitorFactory(ScriptEngine engine)
        {
            _engine = engine;
        }

        public Props Build(IScript script)
        {
            var classProxy = new PythonInteroperabilityClassProxy(_engine,script.ScriptFiles[0]);
            return Props.Create(() => new ClassProxyScriptCompetitor(classProxy));
        }
    }
}