using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Coordination.Messages;
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

                var queryActorProps = Props.Create<TypedQueryActor<AgentCreated>>(
                    selectedPlayers
                    , new RequestCompetitionAgent()
                    , GetRespondSenderResultHandler(obj)
                    , _lobbyConfiguration.CreateCompetitorAgentTimeout);

                Context.ActorOf(queryActorProps);
            });

            Receive<Terminated>(obj =>
            {
                _players.Remove(obj.ActorRef);
            });
        }

        private TypedQueryResultHandler<AgentCreated> GetRespondSenderResultHandler(
            OrderAgents request)
        {
            return ((context, result) =>
            {
                var agents = result.Result.Values
                    .Select(x => x.AgentReference);

                var message = new AgentsOrderCompleted(request.RequestId, agents);
                
                Sender.Tell(message);
            });
        }
    }
}