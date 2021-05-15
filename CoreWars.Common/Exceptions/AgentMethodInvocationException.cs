using System;

namespace CoreWars.Common.Exceptions
{
    public class AgentMethodInvocationException : Exception
    {
        public AgentMethodInvocationException(string message, Exception baseException) : base(message, baseException)
        {
            
        }
    }
}