namespace CoreWars.Player
{
    public static class Diagnostics
    {
        public static string FormatCompetitorFaultedMessage()
        {
            var message = $"Competitor failed because of irreversible agent failure";
            return message;
        }

        public static string FormatCompetitorInvalidMethodCallsExceeded(int maxInvalidMethodCalls)
        {
            var message =
                $"Competitor failed because proxy invalid method calls if {maxInvalidMethodCalls} is exceeded";
            return message;
        }
    }
}