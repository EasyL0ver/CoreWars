using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreWars.Data.Entities;
using CoreWars.Scripting;
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