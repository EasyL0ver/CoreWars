using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions;
using CoreWars.Common.AkkaExtensions.Messages;
using CoreWars.Competition;
using JetBrains.Annotations;

namespace PrisonerDilemma
{
    [UsedImplicitly]
    public class DilemmaCompetition : Competition
    {
        private readonly IDilemmaConfiguration _configuration;
        private readonly Dictionary<IActorRef, int> _playersScore;

        protected override void RunCompetition()
        {
            Self.Tell(new Messages.StartRound());
        }

        protected override CompetitionResult GetResult(IActorRef playerActor)
        {
            var minScore = _playersScore.Values.Min();
            var winners = _playersScore.Where(x => x.Value == minScore).ToList();

            if (winners.Count > 1) return CompetitionResult.Inconclusive;
            if (winners.All(w => w.Key == playerActor))
                return CompetitionResult.Winner;
            
            return CompetitionResult.Loser;
        }

        public DilemmaCompetition(
            IEnumerable<IActorPlayer> competitorActors
            , IDilemmaConfiguration configuration) : base(competitorActors)
        {
            IActorRef actor = null;
            _configuration = configuration;

            _playersScore = Competitors
                .ToDictionary(x => (IActorRef) x, x => 0);

            Receive<Messages.StartRound>(msg =>
            {
                Competitors
                    .QueryFor<bool>(
                        new RunMethodMessage("choose_dilemma")
                        , Context
                        , _configuration.Timeout);
            });
            
            Receive<TypedQueryResult<bool>>(OnDilemmaQueryReceived);
            Receive<TypedQueryResult<Acknowledged>>(OnOpponentMoveAck);
            ReceiveAny(x => Console.WriteLine("cojest"));
        }
        
        //todo customize typed query action on finish
        private void OnOpponentMoveAck(TypedQueryResult<Acknowledged> obj)
        {
            Self.Tell(new Messages.StartRound());
        }

        private void OnDilemmaQueryReceived(TypedQueryResult<bool> obj)
        {
            AdjustScores(obj.Result);
            CurrentIterationCounter++;

            if (CurrentIterationCounter >= _configuration.IterationsCount)
            {
                Conclude();
                return;
            }
            
            NotifyAboutOpponentsMove(obj.Result);
        }

        private static object GetOpponentAction(
            IActorRef playerRef
            , IDictionary<IActorRef, bool> playersActions)
        {
            var opponentsAction = playersActions
                .First(x => x.Key != playerRef)
                .Value;

            return new RunMethodMessage("update_opponent_action", opponentsAction);
        }

        private void NotifyAboutOpponentsMove(
            IDictionary<IActorRef, bool> playersActions)
        {
            Competitors
                .QueryFor<Acknowledged>(
                    (x) => GetOpponentAction(x, playersActions)
                    , Context
                    , _configuration.Timeout);
        }

        private void AdjustScores(IDictionary<IActorRef, bool> playersActions)
        {
            var actionsUniform = playersActions.Values.IsUniform();
            
            playersActions.ForEach(pair =>
            {
                var (playerReference, dilemmaAction) = pair;
                
                var pointsScored = dilemmaAction switch
                {
                    true when actionsUniform => _configuration.BothCooperateScore,
                    false when actionsUniform => _configuration.BothDefectScore,
                    true => _configuration.CooperateScore,
                    false => _configuration.DefectScore,
                };

                _playersScore[playerReference] += pointsScored;
            });
        }


        private int CurrentIterationCounter { get; set; }
    }
}