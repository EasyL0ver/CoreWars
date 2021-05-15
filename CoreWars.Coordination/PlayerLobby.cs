using System;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;
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

            Receive<OrderPlayersSelection>(msg =>
            {
                Sender.Tell(players.Select(lobbyConfiguration.PlayerCount).ToList());
            });

            Receive<OrderAgents>(obj =>
            {
                try
                {
                    var orderProps = LobbyOrder.Props(Sender, Self);
                    Context.ActorOf(orderProps);
                }
                catch (InvalidOperationException e)
                {
                    Sender.Tell(NotEnoughPlayers.Instance);
                }
            });

            Receive<LobbyPlayerTerminated>(obj =>
            {
                players.Remove(obj.ActorRef);
            });
        }

        public static Props Props(ISelectableSet<IActorRef> playerSet, ICompetitionInfo lobbyConfiguration)
        {
            return Akka.Actor.Props.Create(() => new PlayerLobby(playerSet, lobbyConfiguration));
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case AskTargetTerminatedException:
                        case TimeoutException:
                            return Directive.Restart;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
    
    
}