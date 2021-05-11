using System;
using CoreWars.Common;

namespace CoreWars.Player.Messages
{
    public sealed class CompetitorStatusChanged
    {
        public CompetitorStatusChanged(ICompetitorStatus status, Guid competitorId)
        {
            Status = status;
            CompetitorId = competitorId;
        }

        public ICompetitorStatus Status { get;  }
        public Guid CompetitorId { get; }
    }
}