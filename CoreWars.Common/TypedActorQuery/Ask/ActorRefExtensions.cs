using System;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery.Ask
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
    }
}