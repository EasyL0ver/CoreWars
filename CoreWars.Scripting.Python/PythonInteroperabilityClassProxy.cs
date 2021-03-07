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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object InvokeMethod(string methodName, object[] methodParameters, Type expectedResponseType = null)
        {
            dynamic methodResult = Operations.InvokeMember(_instance, methodName, methodParameters);

            if (expectedResponseType == null)
                return null;
            
            return Operations.ConvertTo(methodResult, expectedResponseType);
        }
    }
}