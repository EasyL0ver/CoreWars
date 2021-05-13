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

        public static CompetitorStatus Terminated => new CompetitorStatus(CompetitorState.Faulted, null);

        public CompetitorState State { get; }
        public ICompetitorStatistics Stats { get; }
    }
}