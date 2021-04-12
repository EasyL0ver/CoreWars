using System;

namespace CoreWars.Common.TypedActorQuery.Ask
{
    public class AskTypeMismatchException : Exception
    {
        public object MismatchedResponse { get; }

        public AskTypeMismatchException(string message, object response) : base(message)
        {
            MismatchedResponse = response;
        }
    }
}