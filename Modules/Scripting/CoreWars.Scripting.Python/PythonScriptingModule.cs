using Autofac;
using CoreWars.Common;
using Microsoft.Scripting.Hosting;

namespace CoreWars.Scripting.Python
{
    public class PythonScriptingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context => IronPython.Hosting.Python.CreateEngine())
                .As<ScriptEngine>()
                .SingleInstance();
            
            builder
                .RegisterType<PythonScriptCompetitorFactory>()
                .As<ICompetitorFactory>()
                .Named<ICompetitorFactory>("python");
        }
    }
}