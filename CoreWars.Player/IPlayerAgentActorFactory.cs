using Akka.Actor;

namespace CoreWars.Player
{
    public interface IPlayerAgentActorFactory
    {
        IActorRef Build(IActorContext context);
    }
}