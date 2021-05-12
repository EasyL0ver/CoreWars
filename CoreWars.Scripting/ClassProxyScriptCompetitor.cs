using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public class ClassProxyScriptCompetitor : BaseAgent
    {
        public ClassProxyScriptCompetitor(IInteroperabilityClassProxy interoperabilityClassProxy)
        {
            Receive<RunMethodMessage>(msg =>
            {
                var response = interoperabilityClassProxy
                                   .InvokeMethod(msg.MethodName, msg.MethodParams) 
                               ?? new Acknowledged();

                Sender.Tell(response);
            });
        }

    }
}