using CoreWars.Common;

namespace CoreWars.Player
{
    public class WinRateCounter : ICompetitorStatistics
    {
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        
        public WinRateCounter(int wins, int gamesPlayed)
        {
            Wins = wins;
            GamesPlayed = gamesPlayed;
        }

        public WinRateCounter Copy()
        {
            return new WinRateCounter(Wins, GamesPlayed);
        }
    }
}