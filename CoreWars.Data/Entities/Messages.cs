using System;

namespace CoreWars.Data.Entities
{
    public class Messages
    {
        public class Add<T>
        {
            public Add(T content)
            {
                Content = content;
            }

            public T Content { get; }
        }

        public class GetAll
        {
            public static GetAll Instance => new GetAll();
        }

        public class GetAllForCompetition
        {
            public GetAllForCompetition(string competitionName)
            {
                CompetitionName = competitionName;
            }

            public string CompetitionName { get; }
        }

        public class Subscribe
        {
            public static Subscribe Instance => new Subscribe();
        }

        public class AddedEvent<T>
        {
            public AddedEvent(T addedElement)
            {
                AddedElement = addedElement;
            }

            public T AddedElement { get; }
        }

        public class ScriptCompetitionResult
        {
            public ScriptCompetitionResult(Guid scriptId, Common.CompetitionResult result)
            {
                ScriptId = scriptId;
                Result = result;
            }

            public Guid ScriptId { get; }
            public Common.CompetitionResult Result { get; }
        }

        public sealed class ScriptStatisticsUpdated
        {
            public ScriptStatisticsUpdated(int wins, int gamesPlayed, Guid scriptId)
            {
                Wins = wins;
                GamesPlayed = gamesPlayed;
                ScriptId = scriptId;
            }

            public int Wins { get; }
            public int GamesPlayed { get; }
            public Guid ScriptId { get; }
        }
    }
}