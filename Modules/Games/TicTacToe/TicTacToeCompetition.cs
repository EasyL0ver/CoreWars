using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions;
using CoreWars.Common.AkkaExtensions.Actors.Ask;
using CoreWars.Common.AkkaExtensions.Messages;
using CoreWars.Competition;
using JetBrains.Annotations;

namespace TicTacToe
{
    public class TicTacToeCompetition : Competition
    {
        private int _roundIndex;
        [ItemCanBeNull] private TicTacBoard _gameBoard;
        
        public TicTacToeCompetition(IEnumerable<IActorPlayer> competitorActors) : base(competitorActors)
        {
            _gameBoard = new TicTacBoard();
            _roundIndex = 0;

            Receive<Messages.RunRound>(msg =>
            {
                var activePlayer = Competitors[_roundIndex % 1];
                activePlayer.AskFor<SymbolPlacement>(
                    new RunMethodMessage("place_symbol")
                    , Context
                    , TimeSpan.FromSeconds(5));
            });

            Receive<TypedAskResult<SymbolPlacement>>(msg =>
            {
                _gameBoard.AddSymbol(msg.Sender, msg.Answer);

                if (_gameBoard.IsPlayerWinner(msg.Sender) || _gameBoard.IsFull())
                {
                    Conclude();
                    return;
                }
                
                Self.Tell(new Messages.PropagateGameState());
            });

            Receive<Messages.PropagateGameState>(msg =>
            {
                Competitors
                    .QueryFor<Acknowledged>(
                        (x) => _gameBoard.GetContextPayload(x, 'X', 'O')
                        , Context
                        , TimeSpan.FromSeconds(10));
            });

            Receive<TypedQueryResult<Acknowledged>>(msg =>
            {
                _roundIndex++;
                Self.Tell(new Messages.RunRound());
            });
        }

        protected override void RunCompetition()
        {
            Self.Tell(new Messages.RunRound());
        }

        protected override CompetitionResult GetResult(IActorRef playerActor)
        {
            if (_gameBoard.IsPlayerWinner(playerActor))
                return CompetitionResult.Winner;
            if (_gameBoard.IsFull())
                return CompetitionResult.Inconclusive;
            return CompetitionResult.Loser;
        }
    }
}