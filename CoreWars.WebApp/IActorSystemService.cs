using Akka.Actor;

namespace CoreWars.WebApp
{
    public interface IActorSystemService
    {
        ActorSystem ActorSystem { get; }
    }
}