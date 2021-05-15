using System;
using Akka.Event;

namespace CoreWars.Common
{
    public class GameLog
    {
        public GameLog(string message, LogLevel logLevel)
        {
            Message = message;
            LogLevel = logLevel;
        }

        public Exception Exception { get; set; }
        public string Message { get; }
        public LogLevel LogLevel { get; }
    }
}