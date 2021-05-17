using System;

namespace CoreWars.Common.TypedActorQuery.Query
{
    internal class MessageResponsePair<TResponse>
    {
        public MessageResponsePair(object message)
        {
            Message = message;
        }
            
        public object Message { get; }
        public TResponse Response { get; private set; }
        public bool Responded { get; private set; }

        public void SetResponse(TResponse response)
        {
            if (Responded)
                throw new InvalidOperationException();

            Responded = true;
            Response = response;
        }
    }
}