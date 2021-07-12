using Akka.Actor;
using Akka.Event;

namespace CoreWars.Common
{
    public abstract class BaseAgent : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public BaseAgent()
        {
            Receive<CompetitionResult>(msg =>
            {
                Context.Parent.Tell(msg);
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