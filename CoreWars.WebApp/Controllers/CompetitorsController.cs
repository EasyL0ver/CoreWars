using System;
using System.Linq;
using CoreWars.Data.Entities;
using CoreWars.WebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompetitorsController : ControllerBase
    {
        private readonly IGameService _gameService;
        public CompetitorsController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        [Authorize]
        public void Post([FromBody] Competitor competitor)
        {
            var userId = HttpContext.User.Claims
                .Single(claim => claim.Type == "user-id")
                .Value;
                
            var script = new Script()
            {
                Id = Guid.NewGuid()
                , UserId = Guid.Parse(userId)
                , CompetitionName = competitor.Competition
                , ScriptFiles = new[]{competitor.Code}
                , ScriptType = competitor.Language
                , Name = competitor.Alias
            };
            
            _gameService.AddScript(script);
        }
    }
}