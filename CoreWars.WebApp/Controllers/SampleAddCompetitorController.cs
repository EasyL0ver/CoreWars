using CoreWars.Scripting;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleAddCompetitorController : ControllerBase
    {
        private readonly IClassProxyScriptCompetitorPropsFactory _competitionActorPropsFactory;
        private readonly IActorSystemService _actorSystem;

        public SampleAddCompetitorController(
            IActorSystemService actorSystem, 
            IClassProxyScriptCompetitorPropsFactory competitionActorPropsFactory)
        {
            _actorSystem = actorSystem;
            _competitionActorPropsFactory = competitionActorPropsFactory;
        }


        [HttpPost]
        public void Post(string competitionName, string scriptingLanguage,[FromBody] string code)
        {
            var competitorAgentProps = _competitionActorPropsFactory.Build(code);
            //var playerProps = Competitor.Props(competitorAgentProps, _lobby.LobbyRef);

            //_actorSystem.ActorOf(playerProps);
        }
    }
}