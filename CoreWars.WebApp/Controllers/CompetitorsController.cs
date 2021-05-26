using System;
using System.Collections.Generic;
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
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var msg = new Messages.GetAllForUser(UserId);

            var competitors = await _gameService.ScriptRepository.Ask<List<Script>>(
                msg
                , TimeSpan.FromSeconds(5));

            var model = competitors.Select(
                competitor => new ActiveCompetitor()
            {
                Alias = competitor.Name
                , Code = competitor.ScriptFiles[0]
                , Competition = competitor.CompetitionName
                , Language =  competitor.ScriptType
                , GamesPlayed = competitor.Stats?.GamesPlayed ?? 0
                , GamesWon = competitor.Stats?.Wins ?? 0
                , Status = competitor.FailureInfo == null ? CompetitorState.Active : CompetitorState.Faulted
                , Id = competitor.Id
            });

            return Ok(model);
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