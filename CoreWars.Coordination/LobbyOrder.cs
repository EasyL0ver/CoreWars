using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common.TypedActorQuery.Query;
using CoreWars.Coordination.Messages;
using CoreWars.Player.Messages;

namespace CoreWars.Coordination
{
    public class LobbyOrder : ReceiveActor
    {
        private readonly IActorRef _orderedBy;
        private readonly IReadOnlyList<IActorRef> _players;

        public LobbyOrder(IReadOnlyList<IActorRef> players, IActorRef orderedBy)
        {
            _players = players;
            _orderedBy = orderedBy;

            Receive<TypedQueryResult<IAgentActorRef>>(msg =>
            {
                var agents = msg.Result.Values;
                _orderedBy.Tell(new AgentsOrderCompleted(agents));
            });
        }

        public static Props Props(IEnumerable<IActorRef> players, IActorRef orderedBy)
        {
            return Akka.Actor.Props.Create<LobbyOrder>(() => new LobbyOrder(players.ToList(), orderedBy));
        }

        protected override void PreStart()
        {
            base.PreStart();

            _players.QueryFor<IAgentActorRef>(
                new RequestCreateAgent()
                , Context
                , TimeSpan.FromSeconds(30));
        }
        
        
    }
}