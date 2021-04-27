using System;
using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.Stats
{
    public class GameResultHandler : ReceiveActor
    {
        public GameResultHandler()
        {
            Receive<CompetitionResultMessage>(msg =>
            {

            });
        }
    }
}