using System;

namespace CoreWars.WebApp.Actors
{
    public static class Messages
    {
        public class RegisterCompetitorNotifications
        {
            public RegisterCompetitorNotifications(Guid competitorId, string notificationId)
            {
                CompetitorId = competitorId;
                NotificationId = notificationId;
            }

            public Guid CompetitorId { get; }
            public string NotificationId { get; }
        }
    }
}