namespace CoreWars.Common
{
    public interface ICompetitorStatus : ICompetitorStatistics
    {
        CompetitorState State { get; }
    }

    public enum CompetitorState
    {
        Idle
        , Active
        , Faulted
    }
}