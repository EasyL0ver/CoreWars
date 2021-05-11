using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Player.Messages
{
    public class AddCompetition
    {
        public AddCompetition(IActorRef lobby, ICompetitionInfo competitionInfo)
        {
            Lobby = lobby;
            Info = competitionInfo;
        }

        public IActorRef Lobby { get; }
        public ICompetitionInfo Info { get; }
    }
}