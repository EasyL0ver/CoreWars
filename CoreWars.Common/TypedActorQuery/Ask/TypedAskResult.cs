using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery.Ask
{
    public sealed class TypedAskResult<TAnswer>
    {
        public TypedAskResult(TAnswer answer, IActorRef sender)
        {
            Answer = answer;
            Sender = sender;
        }

        public IActorRef Sender { get; }
        public TAnswer Answer { get; }
    }
}