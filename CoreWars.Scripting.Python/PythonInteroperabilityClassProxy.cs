using System;
using Microsoft.Scripting.Hosting;

namespace CoreWars.Scripting.Python
{
    public class PythonInteroperabilityClassProxy : IInteroperabilityClassProxy
    {
        private readonly ScriptEngine _scriptEngine;
        private readonly ScriptScope _scriptScope;
        private readonly dynamic _instance;

        public PythonInteroperabilityClassProxy(
            string classDefinitionPythonScript
            , string proxiedClassName = "GameController")
        {
            _scriptEngine = IronPython.Hosting.Python.CreateEngine();
            _scriptScope = _scriptEngine.CreateScope();

            _scriptEngine.Execute(classDefinitionPythonScript, _scriptScope);
            
            var proxyType = _scriptScope.GetVariable(proxiedClassName);
            _instance = Operations.CreateInstance(proxyType);
        }

        private ObjectOperations Operations => _scriptEngine.Operations;

        public object InvokeMethod(string methodName, params object[] methodParameters)
        {
            return Operations.InvokeMember(_instance, methodName, methodParameters);
        }
    }
}