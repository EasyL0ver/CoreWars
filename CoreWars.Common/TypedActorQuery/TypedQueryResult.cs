using System.Collections.Generic;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery
{
    public sealed class TypedQueryResult<TQueryResult>
    {
        public TypedQueryResult(IDictionary<IActorRef, TQueryResult> result)
        {
            Result = result;
        }

        public IDictionary<IActorRef, TQueryResult> Result {get;}
    }
}