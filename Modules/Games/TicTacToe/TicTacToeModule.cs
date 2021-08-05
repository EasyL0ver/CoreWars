using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace TicTacToe
{
    public class TicTacToeModule : CompetitionModule
    {
        private class TicTacToeFactory : ICompetitionActorPropsFactory
        {
            public Props Build(IEnumerable<IActorPlayer> competitionParticipants)
            {
                return Props.Create(() => new TicTacToeCompetition(competitionParticipants));
            }
        }
        protected override ICompetitionActorPropsFactory ConfigureFactory()
        {
            return new TicTacToeFactory();
        }

        protected override void ConfigureCompetitionInfo(CompetitionInfo competitionInfo)
        {
            competitionInfo.Name = "tic-tac-toe";
            competitionInfo.PlayerCount = Range<int>.Exactly(2);
            competitionInfo.MaxInstancesCount = 1;
        }
    }
}