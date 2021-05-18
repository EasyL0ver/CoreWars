using Autofac;
using CoreWars.Common;

namespace CoreWars.Scripting.Python
{
    public class PythonScriptingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PythonScriptCompetitorFactory>()
                .As<ICompetitorFactory>()
                .Named<ICompetitorFactory>("python");
        }
    }
}