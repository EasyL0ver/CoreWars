using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.AkkaExtensions.Actors.Ask;
using CoreWars.Common.AkkaExtensions.Actors.Query;
using CoreWars.Common.AkkaExtensions.Messages;

namespace CoreWars.Common.AkkaExtensions
{
    public static class ActorRefExtensions
    {
        public static IActorRef AskFor<T>(
            this IActorRef target
            , object message
            , IActorContext context
            , TimeSpan? timeout = null)
        {
            var props = TypedAsk<T>.Props(target, message, context.Self, timeout);
            return context.ActorOf(props);
        }
        
        public static IActorRef QueryFor<T>(
            this IEnumerable<IActorRef> targets
            , object message
            , IActorContext context
            , TimeSpan? timeout = null)
        {
            var messages = targets.ToDictionary(x => x, y => message);
            return InstantiateQueryActor<T>(messages, context, timeout);
        }

        public static IActorRef QueryFor<T>(
            this IEnumerable<IActorRef> targets
            , Func<IActorRef, object> messageSelector
            , IActorContext context
            , TimeSpan? timeout = null)
        {
            var messages = targets.ToDictionary(x => x, messageSelector);
            return InstantiateQueryActor<T>(messages, context, timeout);
        }

        private static IActorRef InstantiateQueryActor<T>(IDictionary<IActorRef, object> messages,
            IActorContext context,
            TimeSpan? timeout = null)
        {
            var props = TypedQuery<T>.Props(messages, context.Self, timeout);
            var actor = context.ActorOf(props);

            actor.Tell(RunTypedQuery.Instance);

            return actor;
        }
    }
}