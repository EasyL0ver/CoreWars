using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Competition;
using PrisonerDilemma;
using PrisonerDilemma.SampleBot;
using Messages = CoreWars.Competition.Messages;

namespace CoreWars.TestApp
{
    public static class TestApplication
    {
        public static void Run()
        {
            var system = ActorSystem.Create("iot-system");

            var bot1 = system.ActorOf(Props.Create<AlwaysDefectSampleActor>());
            var bot2 = system.ActorOf(Props.Create<AlwaysCooperateSampleActor>());

            var bots = new List<IActorRef>() {bot1, bot2};
            var botsClass = bots.Select(b=> new DilemmaPlayer(new CompetitionAgent(b)));
            IDilemmaConfiguration config = new DefaultDilemmaConfiguration();
            var actualGame = new DilemmaGame(config, botsClass.ToList());

            var game = system.ActorOf(Props.Create<DilemmaCompetitionActor>(config, actualGame));
            
            
            game.Tell(new Messages.RunCompetitionMessage());

            Console.ReadLine();



        }
    }
}