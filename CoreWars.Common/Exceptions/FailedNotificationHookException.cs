using System;

namespace CoreWars.Common.Exceptions
{
    public class FailedNotificationHookException : Exception
    {
        public FailedNotificationHookException(string connectionId, Guid competitorId)
        {
            ConnectionId = connectionId;
            CompetitorId = competitorId;
        }

        public string ConnectionId { get; }
        public Guid CompetitorId { get; }
    }
}