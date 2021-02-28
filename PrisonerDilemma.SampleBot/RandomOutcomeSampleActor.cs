using System;
using Akka.Actor;

namespace PrisonerDilemma.SampleBot
{
    public class RandomOutcomeSampleActor : ReceiveActor
    {
        public RandomOutcomeSampleActor()
        {
            Receive<Messages.PresentDilemmaMessage>(msg =>
            {
                var random = new Random();
                var defect = random.NextDouble() >= 0.5;
                var response = defect ? PrisonerAction.Defect : PrisonerAction.Cooperate;

                Sender.Tell(response);
            });
        }
    }
}