using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoreWars.WebApp.Actors
{
    public class NotificationRoot : ReceiveActor
    {
        private readonly IHubContext<CompetitorNotificationHub> _hubContext;

        public NotificationRoot(
            IHubContext<CompetitorNotificationHub> hubContext)
        {
            _hubContext = hubContext;
            IDictionary<string, IActorRef> subscribedObservers 
                = new Dictionary<string, IActorRef>();

            Receive<Messages.RegisterCompetitorNotifications>(msg =>
            {
                if (!subscribedObservers.ContainsKey(msg.NotificationId))
                {
                    var notifyClientAction = new Func<CompetitorStatus, Task>(
                        (x) => NotifyUser(msg.NotificationId, x));
                    
                    var observerProps = Props.Create(
                        () => new NotificationObserver(msg.CompetitorId, notifyClientAction));

                    subscribedObservers[msg.NotificationId] = Context.ActorOf(observerProps);
                }
                
                Sender.Tell(Acknowledged.Instance);
            });

            Receive<Messages.NotificationUserDisconnected>(msg =>
            {
                if (subscribedObservers.TryGetValue(msg.NotificationId, out var actor))
                {
                    actor.Tell(PoisonPill.Instance);
                    subscribedObservers.Remove(msg.NotificationId);
                }
                
                Sender.Tell(Acknowledged.Instance);
            });
        }

        private async Task NotifyUser(string connectionId, CompetitorStatus status)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Status", status);
        }
    }
}