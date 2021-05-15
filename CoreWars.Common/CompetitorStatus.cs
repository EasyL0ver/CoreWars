namespace CoreWars.Common
{
    public class CompetitorStatus : ICompetitorStatus
    {
        public CompetitorStatus(CompetitorState state, int gamesPlayed, int wins)
        {
            State = state;
            GamesPlayed = gamesPlayed;
            Wins = wins;
        }

        public CompetitorStatus Terminated() => new CompetitorStatus(CompetitorState.Faulted, GamesPlayed, Wins);

        public CompetitorState State { get; }
        public int GamesPlayed { get; }
        public int Wins { get; }
    }
}