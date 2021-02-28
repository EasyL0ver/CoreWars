using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using BotArena;
using BotArena.Common;
using JetBrains.Annotations;

namespace PrisonerDilemma
{
    [UsedImplicitly]
    public class DilemmaCompetitionActor : CompetitionActor<DilemmaContext>
    {
        private readonly IDilemmaConfiguration _configuration;
        private readonly DilemmaGame _game;

        private ICancelable _timeoutCancelable;
        
        public DilemmaCompetitionActor(
            IDilemmaConfiguration configuration
            , DilemmaGame game) 
        {
            _configuration = configuration;
            _game = game;

            Receive<PrisonerAction>(OnActionReceived);
            Receive<Messages.ConcludeRoundMessage>(OnRoundConcluded);
        }
        private void OnRoundConcluded(Messages.ConcludeRoundMessage obj)
        {
            _timeoutCancelable?.Cancel();
            _game.ConcludeRound();
        }

        protected override IReadOnlyList<ICompetitionAgent> Players => _game.Players;
        protected override void RunCompetition(BotArena.Messages.RunCompetitionMessage message)
        {
            RequestChoices();
        }
        private void OnActionReceived(PrisonerAction action)
        {
            var competitorSource = GetPlayerByReference(Sender);
            competitorSource.CurrentAction = action;

            if (!_game.AllPlayersReady)
                return;
            
            Self.Tell(new Messages.ConcludeRoundMessage(), Self);
        }

        private void RequestChoices()
        {
            _game.Players.ForEach(c => c.Reference.Tell(new Messages.PresentDilemmaMessage(), Self));
            
            _timeoutCancelable = new Cancelable(Context.System.Scheduler);
            
            Context.System.Scheduler
                .ScheduleTellOnce(
                    _configuration.ResponseTimeout
                    , Self
                    , new Messages.ConcludeRoundMessage()
                    , Self
                    , _timeoutCancelable );
        }
        private IDilemmaPlayer GetPlayerByReference(IActorRef reference)
        {
            return _game.Players
                .Single(x => x.Reference.Equals(Sender));
        }
        protected override DilemmaContext GetGameContext(IActorRef playerRef)
        {
            var player = GetPlayerByReference(playerRef);
            var opponents = _game.Players.Where(x => x != player).ToList();
            
            return new DilemmaContext()
            {
                Score = player.Score
                , OpponentScore = opponents[0].Score
                , Opponents = opponents
                , OpponentActionLog = opponents[0].ActionLog
            };
        }
    }
}