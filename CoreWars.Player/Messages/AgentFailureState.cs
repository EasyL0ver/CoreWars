using System;

namespace CoreWars.Player.Messages
{
    public class AgentFailureState
    {
        public AgentFailureState(Exception exception)
        {
            Exception = exception;
        }

        public static AgentFailureState Ok() => new AgentFailureState(null);

        public bool Faulted => Exception != null;
        public Exception Exception { get; }
    }
}