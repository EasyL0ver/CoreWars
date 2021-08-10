using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Competition;

namespace CoreWars.Scripting
{
    public abstract class BaseAgent : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = UntypedActor.Context.GetLogger();
        
        public BaseAgent()
        {
            Receive<CompetitorResult>(msg =>
            {
                UntypedActor.Context.Parent.Tell(msg);
            });
        }
  
        
    }
}