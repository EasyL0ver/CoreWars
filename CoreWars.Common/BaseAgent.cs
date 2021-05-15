using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;

namespace CoreWars.Common
{
    public abstract class BaseAgent : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public BaseAgent()
        {
            Receive<CompetitionResultMessage>(msg =>
            {
                var myRef = msg.CompetitionResults.Keys.Single(x => x.Equals(Self));
                var myResult = msg.CompetitionResults[myRef];
                
                Context.Parent.Tell(myResult);
            });
        }

        protected void TraceMessage(string message)
        {
            _logger.Debug(message);
            
            var dto = new GameLog(message, LogLevel.ErrorLevel);
            
            Context.Parent.Tell(dto);
        }
        
    }
}