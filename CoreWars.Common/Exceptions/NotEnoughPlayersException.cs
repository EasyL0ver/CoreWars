using System;

namespace CoreWars.Common.Exceptions
{
    public class NotEnoughPlayersException : Exception
    {
        public NotEnoughPlayersException(int requiredPlayerCount, int actualPlayerCount)
        {
            RequiredPlayerCount = requiredPlayerCount;
            ActualPlayerCount = actualPlayerCount;
        }

        public int RequiredPlayerCount { get; }
        public int ActualPlayerCount { get; }
    }
}