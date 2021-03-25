using System;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public class ScriptCompetitorActor : ReceiveActor
    {
        private readonly IInteroperabilityClassProxy _interoperabilityClassProxy;
        
        public ScriptCompetitorActor(IInteroperabilityClassProxy interoperabilityClassProxy)
        {
            _interoperabilityClassProxy = interoperabilityClassProxy;

            Receive<RunMethodMessage>(msg =>
            {
                var response = _interoperabilityClassProxy
                                   .InvokeMethod(msg.MethodName, msg.MethodParams) 
                               ?? new Acknowledged();

                Sender.Tell(response);
            });
        }

    }
}