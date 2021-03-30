using Akka.Actor;

namespace CoreWars.Coordination.GameSlot
{
    public class QueryData : ICompetitionSlotFSMData
    {
        public QueryData(IActorRef query)
        {
            Query = query;
        }

        public IActorRef Query { get; }
    }
}