using System;
using Akka.Actor;

namespace PrisonerDilemma.SampleBot
{
    public abstract class BaseContextSampleActor : ReceiveActor
    {
        public BaseContextSampleActor()
        {
            Receive<Messages.PresentDilemmaMessage>(msg =>
            {
                Sender.Tell(new BotArena.Messages.RequestContextMessage());
            });

            Receive<DilemmaContext>(context =>
            {
                Sender.Tell(Decide(context));
            });
        }

        protected abstract PrisonerAction Decide(DilemmaContext context);
    }
}