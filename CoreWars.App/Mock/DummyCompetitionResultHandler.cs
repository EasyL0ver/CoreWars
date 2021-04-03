using System;
using Akka.Actor;
using Akka.Event;
using CoreWars.Competition;
using CoreWars.Coordination.Messages;

namespace CoreWars.App.Mock
{
    public class DummyCompetitionResultHandler : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public DummyCompetitionResultHandler()
        {
            Receive<CompetitionResultMessage>(msg =>
            {
                _logger.Info("Received competition result: {0}", msg);
                Sender.Tell(new ResultAcknowledged());
            });
        }
    }
}