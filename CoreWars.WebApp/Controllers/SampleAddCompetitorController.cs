using CoreWars.Scripting;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleAddCompetitorController : ControllerBase
    {
        private readonly ICompetitorFactory _competitionActorFactory;
        private readonly IActorSystemService _actorSystem;

        public SampleAddCompetitorController(
            IActorSystemService actorSystem, 
            ICompetitorFactory competitionActorFactory)
        {
            _actorSystem = actorSystem;
            _competitionActorFactory = competitionActorFactory;
        }


        [HttpPost]
        public void Post(string competitionName, string scriptingLanguage,[FromBody] string code)
        {
            var competitorAgentProps = _competitionActorFactory.Build(code);
            //var playerProps = Competitor.Props(competitorAgentProps, _lobby.LobbyRef);

            //_actorSystem.ActorOf(playerProps);
        }
    }
}