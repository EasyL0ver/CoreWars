using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;
using CoreWars.WebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompetitorsController : CoreWarsController
    {
        private readonly IGameService _gameService;
        public CompetitorsController(IGameService gameService)
        {
            _gameService = gameService;
        }

  

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] Competitor competitor)
        {
            var script = new Script()
            {
                Id = Guid.NewGuid()
                , UserId = UserId
                , CompetitionName = competitor.Competition
                , ScriptFiles = new[]{competitor.Code}
                , ScriptType = competitor.Language
                , Name = competitor.Alias
            };

            await _gameService.ScriptRepository.Ask<Acknowledged>(
                script
                , TimeSpan.FromSeconds(5));

            return Ok(script.Id);
        }
    }
}