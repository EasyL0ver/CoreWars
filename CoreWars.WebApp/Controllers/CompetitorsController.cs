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

            var model = competitors.Select(MapModel);
            return Ok(model);
        }
        
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] Competitor competitor, [FromQuery] string editedCompetitorId)
        {
            var id = Guid.Parse(editedCompetitorId);
            
            var script = new Script()
            {
                Id = id
                , UserId = UserId
                , CompetitionName = competitor.Competition
                , ScriptFiles = new[]{competitor.Code}
                , ScriptType = competitor.Language
                , Name = competitor.Alias
            };

            var message = new Messages.Update<Script>(script);

            var edited = await _gameService.ScriptRepository.Ask<Script>(
                message
                , TimeSpan.FromSeconds(5));

            return Ok(MapModel(edited));
        }
        
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] string deletedCompetitorId)
        {
            var id = Guid.Parse(deletedCompetitorId);
            var message = new Messages.Delete<Script>(id, UserId);

            await _gameService.ScriptRepository.Ask<Acknowledged>(
                message
                , TimeSpan.FromSeconds(5));

            return Ok();
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

            var message = new Messages.Add<Script>(script);

            var addedScript = await _gameService.ScriptRepository.Ask<Script>(
                message
                , TimeSpan.FromSeconds(5));

            return Ok(MapModel(addedScript));
        }

        private static ActiveCompetitor MapModel(Script competitor)
        {
            return new()
            {
                Alias = competitor.Name, Code = competitor.ScriptFiles[0], Competition = competitor.CompetitionName,
                Language = competitor.ScriptType, GamesPlayed = competitor.Stats?.GamesPlayed ?? 0,
                GamesWon = competitor.Stats?.Wins ?? 0,
                Status = competitor.FailureInfo == null ? CompetitorState.Active : CompetitorState.Faulted,
                Id = competitor.Id, Exception = competitor.FailureInfo?.Exception
            };
        }
    }
}