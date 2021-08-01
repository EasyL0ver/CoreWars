using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;
using CoreWars.Scripting;

namespace CoreWars.Player
{
    public class CompetitorsRoot : ReceiveActor
    {
        private readonly List<ICompetitionInfo> _supportedCompetitions = new();
        public CompetitorsRoot(IActorRef scriptRepository, ICompetitorFactory competitorFactory, IActorRef resultRepository)
        {
            Receive<AddCompetition>(msg =>
            {
                var competitionProps = Competition.Props(scriptRepository, msg.Info, msg.Lobby, competitorFactory, resultRepository);
                _supportedCompetitions.Add(msg.Info);
                Context.ActorOf(competitionProps, msg.Info.Name);
            });

            Receive<GetCompetitions>(msg =>
            {
                Sender.Tell(_supportedCompetitions.ToArray());
            });
        }
    }
}