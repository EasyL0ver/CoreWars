using System;

namespace CoreWars.Common.Exceptions
{
    public class AgentFailureException : Exception
    {
        public AgentFailureException(string message, Exception baseException) : base(message, baseException)
        {
            
        }
    }
}