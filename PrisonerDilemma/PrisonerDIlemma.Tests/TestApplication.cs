using System;
using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using CoreWars.Scripting;
using CoreWars.Scripting.Python;
using PrisonerDilemma;
using Messages = CoreWars.Competition.Messages;

namespace PrisonerDIlemma.Tests
{
    public class TestApplication
    {
 
        public static void Run()
        {
            var system = ActorSystem.Create("test-system");

            var alwaysDefectScript = File.ReadAllText("./AlwaysDefectScript.py");
            var alwaysDefectClassProxy = new PythonInteroperabilityClassProxy(alwaysDefectScript);
            var alwaysDefectActorProps = Props.Create<ClassProxyScriptCompetitor>(alwaysDefectClassProxy);
            var alwaysDefectActor = system.ActorOf(alwaysDefectActorProps);
            
            var alwaysCooperateScript = File.ReadAllText("./DefectIfOpponentDefectedScript.py");
            var alwaysCooperateClassProxy = new PythonInteroperabilityClassProxy(alwaysCooperateScript);
            var alwaysCooperateActorProps = Props.Create<ClassProxyScriptCompetitor>(alwaysCooperateClassProxy);
            var alwaysCooperateActor = system.ActorOf(alwaysCooperateActorProps);

            var dilemmaConfig = DilemmaConfiguration.Default();
            var dilemmaProps =
                Props.Create<DilemmaCompetitionActor>(new List<IActorRef>() {alwaysCooperateActor, alwaysDefectActor},
                    dilemmaConfig);

            var dilemmaActor = system.ActorOf(dilemmaProps);
            
            dilemmaActor.Tell(new Messages.RunCompetitionMessage());
            
            
            
            Console.ReadLine();
        }
    }
}