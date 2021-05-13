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

        public class NotificationUserDisconnected
        {
            public NotificationUserDisconnected(string notificationId)
            {
                NotificationId = notificationId;
            }

            public string NotificationId { get; }
        }

        public sealed class GetCurrent
        {
            public static GetCurrent Instance = new GetCurrent();
        }
    }
}