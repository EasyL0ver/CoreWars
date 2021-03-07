using System;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public class ScriptCompetitorActor : ReceiveActor
    {
        private readonly IInteroperabilityClassProxy _interoperabilityClassProxy;
        
        public ScriptCompetitorActor(IInteroperabilityClassProxy interoperabilityClassProxy)
        {
            _interoperabilityClassProxy = interoperabilityClassProxy;
            
            Receive<CompetitionMessage>(OnCompetitionMessageReceived);
        }

        private void OnCompetitionMessageReceived(CompetitionMessage obj)
        {
            //var methodResponse = _agentBehaviour.InvokeMethod(obj.MethodName, obj.Payload, obj.ExpectedResponseType);
        }
    }
}