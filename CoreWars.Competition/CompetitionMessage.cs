using System;

namespace CoreWars.Competition
{
    public class CompetitionMessage
    {
        public string MethodName { get; set; }
        public object Payload { get; set; }
        public Type ExpectedResponseType { get; set; }
    }
}