using Akka.Actor;
using CoreWars.Common;

namespace CoreWars.Player.Messages
{
    public class AddCompetition
    {
        public AddCompetition(IActorRef lobby, ICompetition competition)
        {
            Lobby = lobby;
            Competition = competition;
        }

        public IActorRef Lobby { get; }
        public ICompetition Competition { get; }
    }
}