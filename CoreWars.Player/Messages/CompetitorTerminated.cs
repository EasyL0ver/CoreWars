using System;

namespace CoreWars.Player.Messages
{
    public class CompetitorTerminated
    {
        public CompetitorTerminated(Guid competitorId)
        {
            CompetitorId = competitorId;
        }

        public Guid CompetitorId { get; }
    }
}