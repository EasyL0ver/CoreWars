using System;
using Microsoft.Scripting.Hosting;

namespace CoreWars.Scripting.Python
{
    public class PythonInteroperabilityClassProxy : IInteroperabilityClassProxy
    {
        private readonly string _classDefinitionPythonScript;
        private readonly string _proxiedClassName;
        private readonly ScriptEngine _scriptEngine;
        
        private ScriptScope _scriptScope;
        private dynamic _instance;

        public PythonInteroperabilityClassProxy(
            ScriptEngine pythonScriptEngine
            , string classDefinitionPythonScript
            , string proxiedClassName = "GameController")
        {
            _classDefinitionPythonScript = classDefinitionPythonScript;
            _proxiedClassName = proxiedClassName;
            _scriptEngine = pythonScriptEngine;
        }

        private ObjectOperations Operations => _scriptEngine.Operations;

        public object InvokeMethod(string methodName, params object[] methodParameters)
        {
            return Operations.InvokeMember(_instance, methodName, methodParameters);
        }

        public void Initialize()
        {
            if (_scriptScope is not null)
                throw new InvalidOperationException("Script scope is already initialized!");
            
            _scriptScope = _scriptEngine.CreateScope();

            _scriptEngine.Execute(_classDefinitionPythonScript, _scriptScope);
            
            var proxyType = _scriptScope.GetVariable(_proxiedClassName);
            _instance = Operations.CreateInstance(proxyType);
        }
    }
}