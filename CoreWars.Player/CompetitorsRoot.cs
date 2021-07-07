using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class CompetitorsRoot : ReceiveActor
    {
        private readonly List<ICompetition> _supportedCompetitions = new();
        public CompetitorsRoot(IActorRef scriptRepository, ICompetitorFactory competitorFactory, IActorRef resultRepository)
        {
            Receive<AddCompetition>(msg =>
            {
                var competitionProps = Competition.Props(scriptRepository, msg.Competition.Info, msg.Lobby, competitorFactory, resultRepository);
                _supportedCompetitions.Add(msg.Competition);
                Context.ActorOf(competitionProps, msg.Competition.Info.Name);
            });

            Receive<GetCompetitions>(msg =>
            {
                Sender.Tell(_supportedCompetitions.ToArray());
            });
        }
    }
}