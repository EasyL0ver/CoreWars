using CoreWars.Common;

namespace CoreWars.Coordination.Messages
{
    public sealed class RequestCompetitors
    {
        public RequestCompetitors(string competitionName, Range<int> competitorsAmount)
        {
            CompetitionName = competitionName;
            CompetitorsAmount = competitorsAmount;
        }

        public string CompetitionName { get; }
        public Range<int> CompetitorsAmount { get; }
    
    }
}