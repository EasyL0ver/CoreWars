using System;

namespace TicTacToe
{
    public static class Messages
    {
        public class RunRound {}
        
        public class PropagateGameState {}

        public class TicTacBoardPayload
        {
            public TicTacBoardPayload(char[,] board)
            {
                Board = board;
            }

            public char[,] Board { get; }
        }
    }
}