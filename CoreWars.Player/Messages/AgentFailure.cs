using System;

namespace CoreWars.Player.Messages
{
    public class AgentFailure
    {
        public AgentFailure(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}