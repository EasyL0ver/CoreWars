using JetBrains.Annotations;

namespace CoreWars.Competition
{
    [UsedImplicitly]
    public partial class Messages
    {
        public class RunCompetitionMessage {}

        public class RequestContextMessage {}
        public class RequestCompetitorsInfoMessage {}

        public sealed class CompetitionInconclusive
        {
            public static CompetitionInconclusive Instance => new CompetitionInconclusive();
        }
    }
}