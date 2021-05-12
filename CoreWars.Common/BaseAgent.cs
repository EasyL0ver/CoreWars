using System.Linq;
using Akka.Actor;

namespace CoreWars.Common
{
    public abstract class BaseAgent : ReceiveActor
    {
        public BaseAgent()
        {
            Receive<CompetitionResultMessage>(msg =>
            {
                var myRef = msg.CompetitionResults.Keys.Single(x => x.Equals(Self));
                var myResult = msg.CompetitionResults[myRef];
                
                Context.Parent.Tell(myResult);
            });
        }
    }
}