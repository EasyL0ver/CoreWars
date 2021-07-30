using Akka.Actor;

namespace CoreWars.Game.FSMData
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