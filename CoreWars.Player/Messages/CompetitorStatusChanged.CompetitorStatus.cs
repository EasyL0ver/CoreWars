using CoreWars.Common;

namespace CoreWars.Player.Messages
{
    public class CompetitorStatus : ICompetitorStatus
    {
        public CompetitorStatus(CompetitorState state, ICompetitorStatistics stats)
        {
            State = state;
            Stats = stats;
        }

        public CompetitorState State { get; }
        public ICompetitorStatistics Stats { get; }
    }
}