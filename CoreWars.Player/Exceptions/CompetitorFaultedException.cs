using System;
using Akka.Actor;

namespace CoreWars.Player.Exceptions
{
    public class CompetitorFaultedException : Exception
    {
        public CompetitorFaultedException(IActorRef competitorRef, string message, Exception inner, Guid competitorId) : base(message, inner)
        {
            CompetitorRef = competitorRef;
            CompetitorId = competitorId;
        }

        
        public IActorRef CompetitorRef { get; }
        public Guid CompetitorId { get; }
        
    }
}