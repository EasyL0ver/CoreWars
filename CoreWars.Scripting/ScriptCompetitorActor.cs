using System;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public class ScriptCompetitorActor : UntypedActor
    {
        private readonly IInteroperabilityClassProxy _interoperabilityClassProxy;
        
        public ScriptCompetitorActor(IInteroperabilityClassProxy interoperabilityClassProxy)
        {
            _interoperabilityClassProxy = interoperabilityClassProxy;
        }
        protected override void OnReceive(object message)
        {
            var messageType = message.GetType();
            var methodName = messageType.Name;
            var response = _interoperabilityClassProxy.InvokeMethod(methodName, new object[1] {message});
            
            if(response == null)
                return;
            
            Sender.Tell(response);
        }
    }
}