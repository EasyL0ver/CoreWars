using System;
using System.Collections.Generic;
using System.Linq;
using CoreWars.Common;

namespace PrisonerDilemma
{
    public class DilemmaGame
    {
        private readonly IDilemmaConfiguration _configuration;

        public DilemmaGame(IDilemmaConfiguration configuration, IReadOnlyList<IDilemmaPlayer> players)
        {
            _configuration = configuration;
            Players = players;
        }

        public IReadOnlyList<IDilemmaPlayer> Players { get; }
        public int CurrentIteration { get; private set; } = 0;

        public bool AllPlayersReady => Players.All(x => x.CurrentAction != PrisonerAction.NoAction);

        public void ConcludeRound()
        {
            CurrentIteration += 1;
            UpdateScore();
            
            if(CurrentIteration >= _configuration.IterationsCount)
                throw new InvalidOperationException("placeholder ! game is ended");
        }
        private static bool IsDecisionUniform(IEnumerable<PrisonerAction> source)
        {
            var sourceList = source.ToList();

            return sourceList.All(x => x == sourceList[0]);
        }
        
        private void UpdateScore()
        {
            var actionsUniform = IsDecisionUniform(Players.Select(x => x.CurrentAction));
            
            Players.ForEach(context =>
            {
                var pointsScored = context.CurrentAction switch
                {
                    PrisonerAction.NoAction => 0,
                    PrisonerAction.Cooperate when actionsUniform => _configuration.BothCooperateScore,
                    PrisonerAction.Defect when actionsUniform => _configuration.BothDefectScore,
                    PrisonerAction.Cooperate => _configuration.CooperateScore,
                    PrisonerAction.Defect => _configuration.DefectScore,
                    _ => 0
                };

                context.Score += pointsScored;
                context.ClearAction();
            });
        }

    }
}