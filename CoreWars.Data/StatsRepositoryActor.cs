using System;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;

namespace CoreWars.Data
{
    public class StatsRepositoryActor : ReceiveActor
    {
        private readonly IDataContext _context;

        public StatsRepositoryActor(IDataContext context)
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

            
            Receive<Messages.GetAll>(msg =>
            {
                //todo use stream instead!
                Sender.Tell(_context.Stats.ToList());
            });
            
            Receive<Messages.GetAllForCompetition>(msg =>
            {
                //todo use stream instead!
                Sender.Tell(_context.Stats.Where(stats => stats.Script.CompetitionName == msg.CompetitionName).ToList());
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
                    ScriptId = agentId
                    , Wins = result == CompetitionResult.Winner ? 1 : 0
                    , GamesPlayed = 1
                };
                
                _context.Stats.Add(newStats);
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