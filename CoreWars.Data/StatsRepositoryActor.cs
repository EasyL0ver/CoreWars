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
                    var (agent, result) = value;
                    AdjustScore(agent, result);
                });
                
                Sender.Tell(new ResultAcknowledged());
            });

            
            Receive<Messages.GetAll>(msg =>
            {
                //todo use stream instead!
                Sender.Tell(_context.Stats.ToList());
            });
        }

        private void AdjustScore(IAgentActorRef agentRef, CompetitionResult result)
        {
            var scriptId = agentRef.Info.Id;
            var scriptScore = _context.Stats
                .SingleOrDefault(s => s.ScriptId == scriptId);

            if (scriptScore == null)
            {
                var newStats = new ScriptStatistics()
                {
                    ScriptId = scriptId
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