using System;
using CoreWars.Common;
using CoreWars.Player;

namespace CoreWars.WebApp.Actors.Notification
{
    public class CompetitorStatusCache
    {
        public Guid CompetitorId { get; init; }
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        public CompetitorState State { get; set; }
        public Exception Exception { get; set; }

        public CompetitorStatus GetMessage()
        {
            return new CompetitorStatus(State, GamesPlayed, Wins, CompetitorId, Exception?.GetBaseException().Message);
        }
    }
}