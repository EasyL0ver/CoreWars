using System;
using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Competition;

namespace PrisonerDilemma
{
    public class DilemmaCompetitionActor : CompetitionActor
    {
        private readonly IDilemmaConfiguration _configuration;


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

            Receive<Messages.StartRound>(OnRoundStarted);
        }

        private void OnRoundStarted(Messages.StartRound obj)
        {
            Context.ActorOf<TypedQueryActor<Messages.PresentDilemma, bool>>()
        }

        private int CurrentIterationCounter { get; }
    }
}