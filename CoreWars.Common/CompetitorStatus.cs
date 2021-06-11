using System;

namespace CoreWars.Common
{
    public class CompetitorStatus : ICompetitorStatus
    {
        public CompetitorStatus(CompetitorState state, int gamesPlayed, int wins, Guid competitorId, string exceptionString)
        {
            State = state;
            GamesPlayed = gamesPlayed;
            Wins = wins;
            CompetitorId = competitorId;
            ExceptionString = exceptionString;
        }

        public CompetitorState State { get; }
        public int GamesPlayed { get; }
        public int Wins { get; }
        public Guid CompetitorId { get; }
        public string ExceptionString { get; }
    }
}