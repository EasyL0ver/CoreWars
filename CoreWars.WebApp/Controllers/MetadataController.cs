using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : CoreWarsController
    {
        private readonly IGameService _gameService;

        public MetadataController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        [Route("/languages")]
        public IActionResult GetScriptingLanguages()
        {
            return Ok(_gameService.CompetitorFactory.SupportedCompetitionNames);
        }
        
        [HttpGet]
        [Route("/competitions")]
        public async Task<IActionResult> GetCompetitionsLanguages()
        {
            var competitions = await _gameService.CompetitorsRoot.Ask<ICompetition[]>(
                GetCompetitions.Instance
                , TimeSpan.FromSeconds(5));
            
            return Ok(competitions);
        }
    }
}