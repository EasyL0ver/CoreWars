namespace CoreWars.Common
{
    public interface ICompetitorStatus : ICompetitorStatistics
    {
        CompetitorState State { get; }
    }

    public enum CompetitorState : short
    {
        Inconclusive
        , Active
        , Faulted
    }
}