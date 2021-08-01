using Akka.Actor;
using Akka.Event;
using CoreWars.Common;

namespace CoreWars.Scripting
{
    public abstract class BaseAgent : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = UntypedActor.Context.GetLogger();
        
        public BaseAgent()
        {
            Receive<CompetitionResult>(msg =>
            {
                UntypedActor.Context.Parent.Tell(msg);
            });
        }
  
        
    }
}