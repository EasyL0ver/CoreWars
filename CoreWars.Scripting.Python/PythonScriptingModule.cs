using Autofac;

namespace CoreWars.Scripting.Python
{
    public class PythonScriptingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PythonCompetitorPropsFactory>()
                .As<IClassProxyScriptCompetitorPropsFactory>()
                .Named<IClassProxyScriptCompetitorPropsFactory>("python");
        }
    }
}