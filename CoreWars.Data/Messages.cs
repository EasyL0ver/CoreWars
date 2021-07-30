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
        
        public class Update<T>
        {
            public Update(T content)
            {
                Content = content;
            }

            public T Content { get; }
        }

        public class Delete<T>
        {
            public Delete(Guid deletedObjectId, Guid userRequestedId)
            {
                DeletedObjectId = deletedObjectId;
                UserRequestedId = userRequestedId;
            }

            public Guid DeletedObjectId { get; }
            public Guid UserRequestedId { get; }
        }


        public class GetAllForCompetition
        {
            public GetAllForCompetition(string competitionName)
            {
                CompetitionName = competitionName;
            }

            public string CompetitionName { get; }
        }
        
        public sealed class GetAllForCompetitor
        {
            public GetAllForCompetitor(Guid competitorId)
            {
                CompetitorId = competitorId;
            }

            public Guid CompetitorId { get; }
        }


        public sealed class GetAllForUser
        {
            public Guid UserId { get; }

            public GetAllForUser(Guid userId)
            {
                UserId = userId;
            }
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
        
        public class UpdatedEvent<T>
        {
            public UpdatedEvent(T addedElement)
            {
                AddedElement = addedElement;
            }

            public T AddedElement { get; }
        }
        
        public class DeletedEvent<T>
        {
            public DeletedEvent(T deletedElement)
            {
                DeletedElement = deletedElement;
            }

            public T DeletedElement { get; }
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

        public sealed class ReportScriptFailure
        {
            public ReportScriptFailure(Guid scriptId, Exception exception)
            {
                ScriptId = scriptId;
                Exception = exception;
            }

            public Guid ScriptId { get; }
            public Exception Exception { get; }
            public DateTime FailureDateTime { get; } = DateTime.Now;
        }

        public sealed class ClearScriptError
        {
            public ClearScriptError(Guid scriptId)
            {
                ScriptId = scriptId;
            }

            public Guid ScriptId { get; }
        }
    }
}