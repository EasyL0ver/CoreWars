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
                var messages = selectedPlayers.ToDictionary(x => x, y => (object) new RequestCompetitionAgent());
                var queryProps = TypedQuery<OrderAgents>.Props(
                    messages
                    , Sender
                    , _lobbyConfiguration.CreateCompetitorAgentTimeout);

                Context.ActorOf(queryProps).Tell(RunTypedQuery.Instance);
            });

            Receive<Terminated>(obj =>
            {
                _players.Remove(obj.ActorRef);
            });
        }
    }
}