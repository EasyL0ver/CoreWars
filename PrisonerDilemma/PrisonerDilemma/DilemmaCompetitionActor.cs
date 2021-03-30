﻿using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Common.TypedActorQuery.Query;
using CoreWars.Competition;
using JetBrains.Annotations;

namespace PrisonerDilemma
{
    [UsedImplicitly]
    public class DilemmaCompetitionActor : CompetitionActor
    {
        private readonly IDilemmaConfiguration _configuration;
        private readonly Dictionary<IActorRef, int> _playersScore;

        protected override void RunCompetition()
        {
            Self.Tell(new Messages.StartRound());
        }

        public DilemmaCompetitionActor(
            IEnumerable<IActorRef> competitorActors
            , IDilemmaConfiguration configuration) : base(competitorActors)
        {
            IActorRef actor = null;
            _configuration = configuration;

            _playersScore = Competitors
                .ToDictionary(x => x, x => 0);

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
                AnnounceResult(CompetitionResultMessage.FromScoreboard(_playersScore));
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