using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery
{
    public delegate void TypedQueryResultHandler<T>(IUntypedActorContext queryContext, TypedQueryResult<T> result);
    
    public static class QueryResultStrategy
    {
        public static TypedQueryResultHandler<T> ReportResultToParent<T>()
        {
            return ((context, result) =>
            {
                context.Parent.Tell(result);
            });
        }
    
    }
}