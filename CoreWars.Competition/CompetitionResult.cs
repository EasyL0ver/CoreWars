using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Competition
{
    public enum CompetitionResult
    {
        Inconclusive
        , Winner
        , Loser
    }
    
    public sealed class CompetitionResultMessage
    {
        public CompetitionResultMessage(IReadOnlyDictionary<IActorRef, CompetitionResult> competitionResults)
        {
            CompetitionResults = competitionResults;
        }

        public IReadOnlyDictionary<IActorRef, CompetitionResult> CompetitionResults { get; }

        public static CompetitionResultMessage FromScoreboard(IDictionary<IActorRef, int> scoreBoard)
        {
            var maxScore = scoreBoard.Max(pair => pair.Value);
            var resultsDictionary = scoreBoard
                .ToDictionary(
                    x => x.Key,
                 x => x.Value == maxScore ? CompetitionResult.Winner : CompetitionResult.Loser);

            return new CompetitionResultMessage(resultsDictionary);
        }
    }
}