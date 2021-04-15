using Akka.Actor;

namespace CoreWars.Coordination
{
    public interface ILobby
    {
        IActorRef LobbyRef { get; }
    }

    public class Lobby : ILobby
    {
        public Lobby(IActorRef lobbyRef)
        {
            LobbyRef = lobbyRef;
        }

        public IActorRef LobbyRef { get; }
    }
}