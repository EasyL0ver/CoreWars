using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public class StatsRepository : ReceiveActor
    {
        private readonly IDataContext _context;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public StatsRepository(IDataContext context)
        {
            _context = context;
            Receive<CompetitionResultMessage>(msg =>
            {
                msg.CompetitionResults.ForEach(value =>
                {
                    var (agentId, result) = value;
                    AdjustScore(agentId, result);
                });

                Sender.Tell(new ResultAcknowledged());
            });

            Receive<Messages.ScriptCompetitionResult>(msg =>
            {
                AdjustScore(msg.ScriptId, msg.Result);

                Self.Tell(
                    new Messages.GetAllForCompetitor(msg.ScriptId)
                    , Sender);
            });

            Receive<Messages.GetAllForCompetitor>(msg =>
            {
                var scriptScore = _context.Stats
                    .SingleOrDefault(s => s.ScriptId == msg.CompetitorId);

                if (scriptScore == null)
                {
                    _logger.Info("Cannot find stats for competitor with id: {0}", msg.CompetitorId);
                    return;
                }

                var response = new Messages.ScriptStatisticsUpdated(
                    scriptScore.Wins
                    , scriptScore.GamesPlayed
                    , scriptScore.ScriptId);

                Sender.Tell(response);
            });


            Receive<Messages.GetAllForCompetition>(msg =>
            {
                var stats = _context.Stats
                    .Include(s => s.Script)
                    .Include("Script.User")
                    .Where(s => s.Script.CompetitionName == msg.CompetitionName)
                    .Where(s => !s.Script.IsArchived)
                    .ToList();
                
                Sender.Tell(stats);
            });
        }

        private void AdjustScore(Guid agentId, CompetitionResult result)
        {
            var scriptScore = _context.Stats
                .SingleOrDefault(s => s.ScriptId == agentId);

            if (scriptScore == null)
            {
                var newStats = new ScriptStatistics()
                {
                    ScriptId = agentId, Wins = result == CompetitionResult.Winner ? 1 : 0, GamesPlayed = 1
                };

                _context.Stats.Add(newStats);
                scriptScore = newStats;
            }
            else
            {
                scriptScore.GamesPlayed += 1;

                if (result == CompetitionResult.Winner)
                    scriptScore.Wins += 1;
            }

            _context.Commit();
        }
    }
}