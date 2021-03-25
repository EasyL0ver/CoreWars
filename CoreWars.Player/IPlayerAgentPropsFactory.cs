using Akka.Actor;

namespace CoreWars.Player
{
    public interface IPlayerAgentPropsFactory
    {
        Props Build();
    }
}