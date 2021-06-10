using System;

namespace CoreWars.Common
{
    public class CompetitorStatus : ICompetitorStatus
    {
        public CompetitorStatus(CompetitorState state, int gamesPlayed, int wins, Guid competitorId)
        {
            State = state;
            GamesPlayed = gamesPlayed;
            Wins = wins;
            CompetitorId = competitorId;
        }

        public CompetitorState State { get; }
        public int GamesPlayed { get; }
        public int Wins { get; }
        public Guid CompetitorId { get; }
    }
}