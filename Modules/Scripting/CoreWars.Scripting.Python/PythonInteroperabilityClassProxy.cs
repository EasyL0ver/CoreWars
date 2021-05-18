using Microsoft.Scripting.Hosting;

namespace CoreWars.Scripting.Python
{
    public class PythonInteroperabilityClassProxy : IInteroperabilityClassProxy
    {
        private readonly string _classDefinitionPythonScript;
        private readonly string _proxiedClassName;
        
        private ScriptEngine _scriptEngine;
        private ScriptScope _scriptScope;
        private dynamic _instance;

        public PythonInteroperabilityClassProxy(
            string classDefinitionPythonScript
            , string proxiedClassName = "GameController")
        {
            _classDefinitionPythonScript = classDefinitionPythonScript;
            _proxiedClassName = proxiedClassName;
        }

        private ObjectOperations Operations => _scriptEngine.Operations;

        public object InvokeMethod(string methodName, params object[] methodParameters)
        {
            return Operations.InvokeMember(_instance, methodName, methodParameters);
        }

        public void Initialize()
        {
            _scriptEngine = IronPython.Hosting.Python.CreateEngine();
            _scriptScope = _scriptEngine.CreateScope();

            _scriptEngine.Execute(_classDefinitionPythonScript, _scriptScope);
            
            var proxyType = _scriptScope.GetVariable(_proxiedClassName);
            _instance = Operations.CreateInstance(proxyType);
        }
    }
}