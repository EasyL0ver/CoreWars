using System;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery.Query;
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Player.Messages;

namespace CoreWars.Coordination
{
    public class PlayerLobby : ReceiveActor
    {
        public PlayerLobby(
            ISelectableSet<IActorRef> players
            , ICompetitionInfo lobbyConfiguration)
        {

            Receive<RequestLobbyJoin>(obj =>
            {
                if (players.Contains(Sender))
                    return;

                players.Add(Sender);

                Context.WatchWith(
                    Sender
                    , new LobbyPlayerTerminated(Sender));
            });

            Receive<OrderAgents>(obj =>
            {
                try
                {
                    var selectedPlayers = players.Select(lobbyConfiguration.PlayerCount);
                    var orderProps = LobbyOrder.Props(selectedPlayers, Sender);
                    Context.ActorOf(orderProps);
                }
                catch (InvalidOperationException e)
                {
                    Sender.Tell(NotEnoughPlayers.Instance);
                }
            });

            Receive<Terminated>(obj =>
            {
                players.Remove(obj.ActorRef);
            });
        }

        public static Props Props(ISelectableSet<IActorRef> playerSet, ICompetitionInfo lobbyConfiguration)
        {
            return Akka.Actor.Props.Create(() => new PlayerLobby(playerSet, lobbyConfiguration));
        }
    }
}