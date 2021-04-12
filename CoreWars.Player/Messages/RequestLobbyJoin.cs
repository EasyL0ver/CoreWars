using Akka.Actor;

namespace CoreWars.Player.Messages
{
    public class RequestLobbyJoin
    {
        public static RequestLobbyJoin Instance => new RequestLobbyJoin();
    }
}