namespace CoreWars.WebApp.Model
{
    public class LeaderboardRow
    {
        public string Alias { get; init; }
        public string Creator { get; init; }
        public string Language { get; init; }
        public int GamesPlayed { get; init; }
        public int Wins { get; init; }
        public double WinRate { get; init; }
    }
}