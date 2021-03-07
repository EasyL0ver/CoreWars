using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;
using JetBrains.Annotations;

namespace PrisonerDilemma
{
    [UsedImplicitly]
    public class DilemmaCompetitionActor : CompetitionActor<DilemmaContext>
    {
        private readonly IDilemmaConfiguration _configuration;

        private int _iterationCounter;
        private ICancelable _timeoutCancelable;
        
        public DilemmaCompetitionActor(
            IDilemmaConfiguration configuration)
        {
            _configuration = configuration;
        
            Receive<PrisonerAction>(OnActionReceived);
            Receive<Messages.ConcludeRoundMessage>(OnRoundConcluded);
        }
        
        private void OnRoundConcluded(Messages.ConcludeRoundMessage obj)
        {
            _timeoutCancelable?.Cancel();
            _playerActions.Clear();
            
            if(_iterationCounter > _configuration.IterationsCount)
                throw new InvalidOperationException("placeholder ! game is ended");
            
            UpdateScore();
        }

        private void OnActionReceived(PrisonerAction action)
        {
            _playerActions[Sender] = action;            
        
            if (_playerActions.Count == _players.Count)
                return;
            
            Self.Tell(new Messages.ConcludeRoundMessage(), Self);
        }
        
        private void RequestChoices()
        {
            _players.Keys.ForEach(c => c.Tell(new Messages.PresentDilemmaMessage(), Self));
            
            _timeoutCancelable = new Cancelable(Context.System.Scheduler);
            
            Context.System.Scheduler
                .ScheduleTellOnce(
                    _configuration.ResponseTimeout
                    , Self
                    , new Messages.ConcludeRoundMessage()
                    , Self
                    , _timeoutCancelable );
        }
        
        private static bool IsDecisionUniform(IEnumerable<PrisonerAction> source)
        {
            var sourceList = source.ToList();

            return sourceList.All(x => x == sourceList[0]);
        }
        
        private void UpdateScore()
        {
            var actionsUniform = IsDecisionUniform(_playerActions.Values);
            
            _playerActions.ForEach(pair =>
            {
                var pointsScored = pair.Value switch
                {
                    PrisonerAction.NoAction => 0,
                    PrisonerAction.Cooperate when actionsUniform => _configuration.BothCooperateScore,
                    PrisonerAction.Defect when actionsUniform => _configuration.BothDefectScore,
                    PrisonerAction.Cooperate => _configuration.CooperateScore,
                    PrisonerAction.Defect => _configuration.DefectScore,
                    _ => 0
                };

                _players[pair.Key].Score += pointsScored;
            });
        }

        private readonly Dictionary<IActorRef, IDilemmaPlayer> _players;
        private readonly Dictionary<IActorRef, PrisonerAction> _playerActions;

        public DilemmaCompetitionActor(
            Dictionary<IActorRef, IDilemmaPlayer> players)
        {
            _players = players;
        }

        protected override void RunCompetition()
        {
            RequestChoices();
        }
    }
}