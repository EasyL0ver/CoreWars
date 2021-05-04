using Akka.Actor;

namespace CoreWars.Common
{
    public interface IAgentActorRef : IActorRef
    {
        IUser Creator { get; }
        IScriptInfo Info { get; }
    }
}