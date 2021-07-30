using System;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions;
using CoreWars.Common.Exceptions;
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Player.Exceptions;
using CoreWars.Player.Messages;
using CompetitorTerminated = CoreWars.Coordination.Messages.CompetitorTerminated;

namespace CoreWars.Coordination
{
    public class CompetitorLobby : ReceiveActor
    {
        public CompetitorLobby(
            ISelectableSet<IActorRef> competitors
            , ICompetitionInfo lobbyConfiguration)
        {
            Receive<RequestLobbyJoin>(obj =>
            {
                if (competitors.Contains(Sender))
                    return;

                competitors.Add(Sender);

                Context.WatchWith(
                    Sender
                    , new CompetitorTerminated(Sender));
            });

            Receive<RequestLobbyQuit>(obj =>
            {
                Context.Unwatch(Sender);
                competitors.Remove(Sender);
            });

            Receive<OrderCompetitorsSelection>(msg =>
            {
                try
                {
                    Sender.Tell(competitors.Select(lobbyConfiguration.PlayerCount).ToList());
                }
                catch (NotEnoughPlayersException e)
                {
                    Sender.Tell(NotEnoughPlayers.Instance);
                }
            });

            Receive<OrderAgents>(obj =>
            {
                var orderProps = LobbyOrder.Props(Sender, Self);
                Context.ActorOf(orderProps);
            });

            Receive<CompetitorTerminated>(obj => { competitors.Remove(obj.ActorRef); });
        }

        public static Props Props(ISelectableSet<IActorRef> playerSet, ICompetitionInfo lobbyConfiguration)
        {
            return Akka.Actor.Props.Create(() => new CompetitorLobby(playerSet, lobbyConfiguration));
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
                        case CompetitorFaultedException cfe:
                            Self.Tell(RequestLobbyQuit.Instance, cfe.CompetitorRef);
                            return Directive.Restart;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
}