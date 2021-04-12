using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using CoreWars.Competition;
using CoreWars.Player;
using CoreWars.Scripting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleAddCompetitorController : ControllerBase
    {
        private readonly DITest _test;
        private readonly IClassProxyScriptCompetitorPropsFactory _competitionActorPropsFactory;
        private readonly ActorSystem _actorSystem;
        private readonly IDependencyResolver _resolver;

        public SampleAddCompetitorController(
            DITest test, ActorSystem actorSystem, IClassProxyScriptCompetitorPropsFactory competitionActorPropsFactory, IDependencyResolver resolver)
        {
            _test = test;
            _actorSystem = actorSystem;
            _competitionActorPropsFactory = competitionActorPropsFactory;
            _resolver = resolver;
        }

        [HttpGet]
        public string Get()
        {
            
            return _test.GetMessage;
        }
        [HttpPost]
        public void Post(string competitionName, string scriptingLanguage, string code)
        {
            var competitorAgentProps = _competitionActorPropsFactory.Build(code);
        }
    }
}