using System;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common.TypedActorQuery.Query;
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Player.Messages;

namespace CoreWars.Coordination
{
    public class PlayerLobby : ReceiveActor
    {
        private readonly ISelectableSet<IActorRef> _players;
        private readonly ILobbyConfig _lobbyConfiguration;

        public PlayerLobby(
            ISelectableSet<IActorRef> players
            , ILobbyConfig lobbyConfiguration)
        {
            _players = players;
            _lobbyConfiguration = lobbyConfiguration;

            Receive<AddPlayer>(obj =>
            {
                if (_players.Contains(obj.AddedPlayerReference))
                    return;

                _players.Add(obj.AddedPlayerReference);

                Context.WatchWith(
                    obj.AddedPlayerReference
                    , new LobbyPlayerTerminated(obj.AddedPlayerReference));
            });

            Receive<OrderAgents>(obj =>
            {
                var selectedPlayers = _players.Select(_lobbyConfiguration.PlayerCount);
                var messages = selectedPlayers.ToDictionary(x => x, y => (object) new RequestCreateAgent());


                var transform = new Func<TypedQueryResult<AgentCreated>, AgentsOrderCompleted>(input =>
                {
                    return new AgentsOrderCompleted(input.Result.Values.Select(x => x.AgentReference));
                });

                var transformActorProps =
                    Akka.Actor.Props.Create(
                        () => new MessageTransform<TypedQueryResult<AgentCreated>, AgentsOrderCompleted>(transform,
                            Sender));

                var transformActor = Context.ActorOf(transformActorProps);
                
                var queryProps = TypedQuery<AgentCreated>.Props(
                    messages
                    , transformActor
                    , _lobbyConfiguration.CreateCompetitorAgentTimeout);
                

                Context.ActorOf(queryProps).Tell(RunTypedQuery.Instance);
            });

            Receive<Terminated>(obj =>
            {
                _players.Remove(obj.ActorRef);
            });
        }

        public static Props Props(ISelectableSet<IActorRef> playerSet, ILobbyConfig lobbyConfiguration)
        {
            return Akka.Actor.Props.Create(() => new PlayerLobby(playerSet, lobbyConfiguration));
        }
    }
}