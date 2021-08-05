using System;
using System.IO;
using Autofac;
using CoreWars.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Scripting.Hosting;
using TicTacToe.Imports;

namespace CoreWars.Scripting.Python
{
    [UsedImplicitly]
    public class PythonScriptingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context =>
                {
                    var config = context.Resolve<IConfiguration>();
                    var ironPythonPath = config["IronPythonPath"];
                    var libraryPath = Path.Combine(ironPythonPath, "Lib");

                    if (!Directory.Exists(libraryPath))
                        throw new InvalidOperationException("iron python library folder does not exist!");
                    
                    var engine = IronPython.Hosting.Python.CreateEngine();

                    var engineSearchPaths = engine.GetSearchPaths();
                    engineSearchPaths.Add(libraryPath);
                    engine.SetSearchPaths(engineSearchPaths);
                    
                    //todo this is a hack!
                    engine.Runtime.LoadAssembly(typeof(SymbolPlacement).Assembly);

                    return engine;
                })
                .As<ScriptEngine>()
                .SingleInstance();
            
            builder
                .RegisterType<PythonScriptCompetitorFactory>()
                .As<ICompetitorFactory>()
                .Named<ICompetitorFactory>("python");
        }
    }
}