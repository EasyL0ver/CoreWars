using System;

namespace CoreWars.Coordination.Messages
{
    public sealed class OrderAgents
    {
        public Guid RequestId { get; }

        public OrderAgents()
        {
            RequestId = Guid.NewGuid();;
        }

        public static OrderAgents Instance => new OrderAgents();
    }
}