using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWars.Common
{
    public sealed class CompetitionResultMessage
    {
        public CompetitionResultMessage(IReadOnlyDictionary<Guid, CompetitionResult> competitionResults)
        {
            CompetitionResults = competitionResults;
        }

        public IReadOnlyDictionary<Guid, CompetitionResult> CompetitionResults { get; }

        public static CompetitionResultMessage FromScoreboard(IDictionary<Guid, int> scoreBoard)
        {
            var maxScore = scoreBoard.Max(pair => pair.Value);
            var resultsDictionary = scoreBoard
                .ToDictionary(
                    x => x.Key,
                 x => x.Value == maxScore ? CompetitionResult.Winner : CompetitionResult.Loser);

            return new CompetitionResultMessage(resultsDictionary);
        }

        public override string ToString()
        {
            var winnersCount = CompetitionResults.Values.Count(x => x == CompetitionResult.Winner);
            var losersCount = CompetitionResults.Values.Count(x => x == CompetitionResult.Loser);
            return $"Competition results with: {winnersCount} winners and {losersCount} losers";
        }
    }
}