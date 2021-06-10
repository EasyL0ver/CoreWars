using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.WebApp.Actors.Notification;
using CoreWars.WebApp.Hubs;

namespace CoreWars.WebApp.Actors
{
    public class NotificationRoot : ReceiveActor
    {
        public NotificationRoot(
            Func<Messages.RegisterCompetitorNotifications, Props> watcherFactory)
        {
            IDictionary<string, Dictionary<Guid, IActorRef>> subscribedObservers 
                = new Dictionary<string, Dictionary<Guid, IActorRef>>();

            Receive<Messages.RegisterCompetitorNotifications>(msg =>
            {
                if (!subscribedObservers.ContainsKey(msg.NotificationId))
                    subscribedObservers[msg.NotificationId] = new Dictionary<Guid, IActorRef>();

                var clientSubscriptions = subscribedObservers[msg.NotificationId];
                
                if (!clientSubscriptions.ContainsKey(msg.CompetitorId))
                {
                    var observerProps = watcherFactory.Invoke(msg);
                    clientSubscriptions[msg.CompetitorId] = Context.ActorOf(observerProps);
                }
                
                Sender.Tell(Acknowledged.Instance);
                
            });

            Receive<Messages.NotificationUserDisconnected>(msg =>
            {
                if (subscribedObservers.TryGetValue(msg.NotificationId, out var actorRefs))
                {
                    actorRefs.Values.ForEach(r => r.Tell(PoisonPill.Instance));
                    subscribedObservers.Remove(msg.NotificationId);
                }
                
                Sender.Tell(Acknowledged.Instance);
            });
        }
    }
}