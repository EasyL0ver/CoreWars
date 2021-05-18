using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class CompetitorsRoot : ReceiveActor
    {
        public CompetitorsRoot(IActorRef scriptRepository, ICompetitorFactory competitorFactory, IActorRef resultRepository)
        {
            Receive<AddCompetition>(msg =>
            {
                var competitionProps = Competition.Props(scriptRepository, msg.Info, msg.Lobby, competitorFactory, resultRepository);
                Context.ActorOf(competitionProps, msg.Info.Name);
            });
        }
    }
}