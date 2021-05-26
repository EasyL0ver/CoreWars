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
            IDictionary<string, IActorRef> subscribedObservers 
                = new Dictionary<string, IActorRef>();

            Receive<Messages.RegisterCompetitorNotifications>(msg =>
            {
                if (!subscribedObservers.ContainsKey(msg.NotificationId))
                {
                    var observerProps = watcherFactory.Invoke(msg);
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
    }
}