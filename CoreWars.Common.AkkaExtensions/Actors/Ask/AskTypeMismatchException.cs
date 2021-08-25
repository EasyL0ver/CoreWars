using System;

namespace CoreWars.Common.AkkaExtensions.Actors.Ask
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