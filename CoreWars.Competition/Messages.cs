using JetBrains.Annotations;

namespace CoreWars.Competition
{
    [UsedImplicitly]
    public class Messages
    {
        public class RunCompetitionMessage {}

        public sealed class Acknowledged
        {
        }

        public class RequestContextMessage {}
        public class RequestCompetitorsInfoMessage {}
    }
}