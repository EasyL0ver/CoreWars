using CoreWars.Common;

namespace CoreWars.WebApp.Actors.Notification
{
    public class CompetitorStatusCache
    {
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        public CompetitorState State { get; set; }

        public CompetitorStatus GetMessage()
        {
            return new CompetitorStatus(State, GamesPlayed, Wins);
        }
    }
}