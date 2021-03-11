using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery
{
    public sealed class TypedQueryTimeoutException : Exception
    {
        public TypedQueryTimeoutException(IEnumerable<IActorRef> timedOutActors)
        {
            TimedOutActors = timedOutActors.ToList();
        }

        public IReadOnlyList<IActorRef> TimedOutActors { get; }
    }
}