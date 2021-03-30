namespace CoreWars.Common.TypedActorQuery.Query
{
    internal class MessageResponsePair<TResponse>
    {
        public MessageResponsePair(object message)
        {
            Message = message;
        }
            
        public object Message { get; }
        public TResponse Response { get; set; }
    }
}