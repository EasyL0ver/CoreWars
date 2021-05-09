using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Data.Entities;
using CoreWars.WebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWars.WebApp.Controllers
{
    [ApiController, Route("[controller]")]
    public class LeaderboardController : CoreWarsController
    {
        private readonly IGameService _gameService;

        public LeaderboardController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<JsonResult> GetLeaderBoard([FromQuery] string competition)
        {
            var stats = await _gameService.ResultsHandler.Ask<IEnumerable<ScriptStatistics>>(
                new Messages.GetAllForCompetition(competition)
                , TimeSpan.FromSeconds(5));

            var leaderboard = stats
                .Select(MapRow)
                .OrderByDescending(row => row.WinRate);

            return Json(leaderboard);
        }

        private static LeaderboardRow MapRow(ScriptStatistics statistics)
        {
            return new()
            {
                Alias = statistics.Script.Name
                , Creator = statistics.Script.User.EmailAddress
                , Wins = statistics.Wins
                , GamesPlayed = statistics.GamesPlayed
                , WinRate = statistics.Wins / (double) statistics.GamesPlayed
                , Language = statistics.Script.ScriptType
            };
        }
    }
}