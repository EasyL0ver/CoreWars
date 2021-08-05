using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Akka.Actor;
using JetBrains.Annotations;
using TicTacToe.Imports;

namespace TicTacToe
{
    public class TicTacBoard
    {
        public IActorRef[,] Tiles { get; } = new IActorRef[3, 3];

        public void AddSymbol(IActorRef player, SymbolPlacement placement)
        {
            if (Tiles[placement.X, placement.Y] != null)
                throw new InvalidOperationException("Invalid player move!");

            Tiles[placement.X, placement.Y] = player;
        }

        public bool IsFull()
        {
            return Tiles.Cast<IActorRef>().All(tile => tile != null);
        }

        public bool IsPlayerWinner(IActorRef player)
        {
            var checkCombinations = new List<IActorRef[]>();
            for (int i = 0; i < 3; i++)
            {
                checkCombinations.Add(GetColumn(i));
                checkCombinations.Add(GetRow(i));
            }
            
            checkCombinations.Add(GetLeftVertical());
            checkCombinations.Add(GetRightVertical());

            return checkCombinations
                .Any(combination => combination.All(element => element != null && element.Equals(player)));

        }

        public TicTacBoardPayload GetContextPayload(IActorRef player, char playerSign, char opponentSign)
        {
            string[,] array = new string[3, 3];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var tileContent = Tiles[x, y];
                    string sign;

                    if (tileContent == null)
                    {
                        sign = null;
                    }
                    else if (tileContent.Equals(player))
                    {
                        sign = playerSign.ToString();
                    }
                    else
                    {
                        sign = opponentSign.ToString();
                    }

                    array[x, y] = sign;
                }
            }
            return new TicTacBoardPayload(array);
        }

        private IActorRef[] GetColumn(int columnNumber)
        {
            return Enumerable.Range(0, Tiles.GetLength(0))
                .Select(x => Tiles[x, columnNumber])
                .ToArray();
        }

        private IActorRef[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, Tiles.GetLength(1))
                .Select(x => Tiles[rowNumber, x])
                .ToArray();
        }

        private IActorRef[] GetLeftVertical()
        {
            return new[] {Tiles[0, 0], Tiles[1, 1], Tiles[2, 2]};
        }

        private IActorRef[] GetRightVertical()
        {
            return new[] {Tiles[0, 2], Tiles[1, 1], Tiles[2, 0]};
        }

    }
}