using System;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions.Messages;
using CoreWars.Common.Exceptions;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public class ClassProxyScriptCompetitor : BaseAgent
    {
        private readonly IInteroperabilityClassProxy _interoperabilityClassProxy;

        public ClassProxyScriptCompetitor(IInteroperabilityClassProxy interoperabilityClassProxy)
        {
            _interoperabilityClassProxy = interoperabilityClassProxy;
            
            Receive<RunMethodMessage>(msg =>
            {
                try
                {
                    var response = interoperabilityClassProxy
                                       .InvokeMethod(msg.MethodName, msg.MethodParams) 
                                   ?? new Acknowledged();
                    
                    Sender.Tell(response);
                    TraceMessage(FormatLogMessage(msg, response));
                }
                catch (Exception e)
                {
                    var message = $"Exception thrown while {msg}";
                    throw new AgentMethodInvocationException(message, e);
                }
            });
        }

        protected override void PreStart()
        {
            try
            {
                _interoperabilityClassProxy.Initialize();
            }
            catch (Exception e)
            {
                throw new AgentFailureException($"Failed to initialize class proxy", e);
            }
        }

        private string FormatLogMessage(RunMethodMessage message, object response)
        {
            return response is Acknowledged
                ? $"Executed {message}"
                : $"Executed {message} with response {response}";
        }

    }
}