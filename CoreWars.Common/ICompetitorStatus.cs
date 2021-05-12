namespace CoreWars.Common
{
    public interface ICompetitorStatus
    {
        CompetitorState State { get; }
        ICompetitorStatistics Stats { get; }
    }

    public enum CompetitorState
    {
        Idle
        , Active
        , Faulted
    }
}