using System;
using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.WebApp.Actors.Notification
{
    public class ObserverSubscription
    {
        public ObserverSubscription(IActorRef actor)
        {
            Actor = actor;
        }

        public IActorRef Actor { get; }
        public int FailureCount { get; set; } = 0;
    }
}