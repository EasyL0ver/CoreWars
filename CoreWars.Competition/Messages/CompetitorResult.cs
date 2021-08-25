using CoreWars.Common;

namespace CoreWars.Competition
{
    public class CompetitorResult
    {
        public CompetitorResult(CompetitionResult result, int score)
        {
            Result = result;
            Score = score;
        }

        public CompetitionResult Result { get; }
        public int Score { get; }

        public static CompetitorResult FromResult(CompetitionResult result)
        {
            var score = 0;
            if (result == CompetitionResult.Winner) score++;

            return new CompetitorResult(result, score);
        }
    }
}