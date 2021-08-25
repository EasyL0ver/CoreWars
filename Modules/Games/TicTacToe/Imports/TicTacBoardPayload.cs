using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Imports
{
    public class TicTacBoardPayload
    {
        public TicTacBoardPayload(string[,] board)
        {
            Board = board;
            
        }

        public string[,] Board { get; }
    }
}