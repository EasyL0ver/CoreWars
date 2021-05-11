using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class CompetitorsRoot : ReceiveActor
    {
        public CompetitorsRoot(IActorRef scriptRepository, ICompetitorFactory competitorFactory)
        {
            Receive<AddCompetition>(msg =>
            {
                var competitionProps = Competition.Props(scriptRepository, msg.Info, msg.Lobby, competitorFactory);
                Context.ActorOf(competitionProps, msg.Info.Name);
            });
        }
    }
}