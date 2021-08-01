using CoreWars.Common;
using CoreWars.Player;

namespace CoreWars.WebApp.Model
{
    public class ActiveCompetitor : Competitor
    {
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public CompetitorState Status { get; set; }
    }
}