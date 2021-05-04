using Akka.Actor;
using Akka.Util;
using CoreWars.Common;

namespace CoreWars.Player
{
    public class AgentActorRef : IAgentActorRef
    {
        private readonly IActorRef _innerRef;

        public AgentActorRef(IActorRef innerRef, IUser creator,  IScriptInfo info)
        {
            _innerRef = innerRef;
            Creator = creator;
            Info = info;
        }

        public void Tell(object message, IActorRef sender)
        {
            _innerRef.Tell(message, sender);
        }

        public bool Equals(IActorRef? other)
        {
            return _innerRef.Equals(other);
        }

        public int CompareTo(IActorRef? other)
        {
            return _innerRef.CompareTo(other);
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            return _innerRef.ToSurrogate(system);
        }

        public int CompareTo(object? obj)
        {
            return _innerRef.CompareTo(obj);
        }

        public override int GetHashCode()
        {
            return _innerRef.GetHashCode();
        }

        public ActorPath Path => _innerRef.Path;
        public IUser Creator { get; }
        public IScriptInfo Info { get; }
    }
}