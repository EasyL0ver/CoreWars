using System;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery.Query
{
    public class MessageTransform<TIn, TOut> : ReceiveActor
    {
        public MessageTransform(
            Func<TIn, TOut> transformFunction
            , IActorRef target)
        {
            Receive<TIn>(msg =>
            {
                target.Tell(transformFunction(msg));
                Context.Stop(Self);
            });
        }

        public static Props Props<TFIn, TFOut>(Func<TFIn, TFOut> transform, IActorRef target)
        {
            return Akka.Actor.Props.Create(() => new MessageTransform<TFIn, TFOut>(transform, target));
        }
    }
}