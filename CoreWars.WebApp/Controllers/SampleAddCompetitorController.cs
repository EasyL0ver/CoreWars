using System;
using CoreWars.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleAddCompetitorController : ControllerBase
    {
        private readonly IGameService _gameService;
        public SampleAddCompetitorController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public void Post(string competitionName, string scriptingLanguage,[FromBody] string code)
        {
            var script = new Script()
            {
                Id = Guid.NewGuid()
                , CompetitionName = competitionName
                , ScriptFiles = new[]{code}
                , ScriptType = scriptingLanguage
            };
            
            _gameService.AddScript(script);
        }
    }
}