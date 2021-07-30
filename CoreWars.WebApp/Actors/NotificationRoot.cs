using System;
using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions.Messages;
using CoreWars.Common.Exceptions;
using CoreWars.WebApp.Actors.Notification;

namespace CoreWars.WebApp.Actors
{
    public class NotificationRoot : ReceiveActor
    {
        private readonly IDictionary<string, Dictionary<Guid, ObserverSubscription>> _subscribedObservers =
            new Dictionary<string, Dictionary<Guid, ObserverSubscription>>();
        
        public NotificationRoot(
            Func<Messages.RegisterCompetitorNotifications, Props> watcherFactory)
        {
            Receive<Messages.RegisterCompetitorNotifications>(msg =>
            {
                if (!_subscribedObservers.ContainsKey(msg.NotificationId))
                    _subscribedObservers[msg.NotificationId] = new Dictionary<Guid, ObserverSubscription>();

                var clientSubscriptions = _subscribedObservers[msg.NotificationId];
                
                if (!clientSubscriptions.ContainsKey(msg.CompetitorId))
                {
                    var observerProps = watcherFactory.Invoke(msg);
                    clientSubscriptions[msg.CompetitorId] = new ObserverSubscription(Context.ActorOf(observerProps));
                }
                
                Sender.Tell(Acknowledged.Instance);
                
            });

            Receive<Messages.NotificationUserDisconnected>(msg =>
            {
                if (_subscribedObservers.TryGetValue(msg.NotificationId, out var actorRefs))
                {
                    actorRefs.Values.ForEach(r => r.Actor.Tell(PoisonPill.Instance));
                    _subscribedObservers.Remove(msg.NotificationId);
                }
                
                Sender.Tell(Acknowledged.Instance);
            });
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case FailedNotificationHookException hookException:
                            var subscription =
                                _subscribedObservers[hookException.ConnectionId][hookException.CompetitorId];

                            if (subscription.FailureCount < 4)
                            {
                                subscription.FailureCount++;
                                return Directive.Restart;
                            }
                            
                            var registrations = _subscribedObservers[hookException.ConnectionId];
                            registrations.Remove(hookException.CompetitorId);
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                }, false);
        }
    }
}