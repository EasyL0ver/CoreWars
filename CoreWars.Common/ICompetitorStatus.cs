namespace CoreWars.Common
{
    public interface ICompetitorStatus
    {
        CompetitorState State { get; }
        ICompetitorStatistics Stats { get; }
    }

    public interface ICompetitorStatistics
    {
        int GamesPlayed { get; }
        int Wins { get; }
    }

    public enum CompetitorState
    {
        Idle
        , Active
        , Faulted
    }
}