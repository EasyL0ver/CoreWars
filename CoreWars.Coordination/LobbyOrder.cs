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
        private readonly IActorRef _playersSource;


        public LobbyOrder(IActorRef orderedBy, IActorRef playersSource)
        {
            _orderedBy = orderedBy;
            _playersSource = playersSource;
            
            
            WaitingForPlayersSelection();
        }

        public static Props Props(IActorRef orderedBy, IActorRef playersSource)
        {
            return Akka.Actor.Props.Create(() => new LobbyOrder(orderedBy, playersSource));
        }

        protected override void PreStart()
        {
            _playersSource.Tell(OrderPlayersSelection.Instance);
            base.PreStart();
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                _ => Directive.Escalate);
        }

        private void WaitingForPlayersSelection()
        {
            Receive<IEnumerable<IActorRef>>(players =>
            {
                players.QueryFor<IAgentActorRef>(
                    new RequestCreateAgent()
                    , Context
                    , TimeSpan.FromSeconds(30));
                
                Become(WaitingForQueryResponse);
            });
        }

        private void WaitingForQueryResponse()
        {
            Receive<TypedQueryResult<IAgentActorRef>>(msg =>
            {
                var agents = msg.Result.Values;
                _orderedBy.Tell(new AgentsOrderCompleted(agents));
            });
        }
    }
}