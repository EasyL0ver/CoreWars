using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;

namespace CoreWars.WebApp.Mock
{
    public class DummyCompetitor : ReceiveActor
    {
        public DummyCompetitor()
        {
            Receive<CompetitionResultMessage>(msg =>
            {

            });
            ReceiveAny(msg =>
            {
                Sender.Tell(new Acknowledged());
            });
        }
    }
}