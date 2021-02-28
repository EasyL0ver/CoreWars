using System;
using Akka.Actor;

namespace PrisonerDilemma.SampleBot
{
    public class AlwaysDefectSampleActor : ReceiveActor
    {
        public AlwaysDefectSampleActor()
        {
            Receive<Messages.PresentDilemmaMessage>(msg =>
            {
                Sender.Tell(PrisonerAction.Defect);
            });
        }
    }
    
    public class AlwaysCooperateSampleActor : ReceiveActor
    {
        public AlwaysCooperateSampleActor()
        {
            Receive<Messages.PresentDilemmaMessage>(msg =>
            {
                Sender.Tell(PrisonerAction.Cooperate);
            });
        }
    }
}