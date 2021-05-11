using System;
using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Player.Messages;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoreWars.WebApp.Actors
{
    public class CompetitorStatusNotificationActor : ReceiveActor
    {
        private readonly IHubContext<CompetitorNotificationHub> _hubContext;
        private readonly Dictionary<Guid, string> _notificationRegistrations;


        public CompetitorStatusNotificationActor(IHubContext<CompetitorNotificationHub> hubContext)
        {
            _hubContext = hubContext;
            _notificationRegistrations = new Dictionary<Guid, string>();

            Receive<Messages.RegisterCompetitorNotifications>(msg =>
            {
                _notificationRegistrations[msg.CompetitorId] = msg.NotificationId;
            });

            Receive<CompetitorStatusChanged>(msg =>
            {
                if (!_notificationRegistrations.ContainsKey(msg.CompetitorId))
                    return;

                var connectionId = _notificationRegistrations[msg.CompetitorId];
                
                _hubContext.Clients.User(connectionId).SendAsync("gwno");
            });

        }

   
    }
}