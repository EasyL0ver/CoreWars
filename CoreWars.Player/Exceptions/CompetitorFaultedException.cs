using System;

namespace CoreWars.Player.Exceptions
{
    public class CompetitorFaultedException : Exception
    {
        public CompetitorFaultedException(Guid competitorId, string message, Exception inner) : base(message, inner)
        {
            CompetitorId = competitorId;
        }

        public Guid CompetitorId { get; }
        
    }
}