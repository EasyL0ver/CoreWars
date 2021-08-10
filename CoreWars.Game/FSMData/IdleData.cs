using System;

namespace CoreWars.Game.FSMData
{
    public enum IdleReason
    {
        JustStarted
        , NotEnoughPlayers
    }
    
    public class IdleData : ICompetitionSlotFSMData
    {
        public IdleData(IdleReason reason)
        {
            Reason = reason;
            RestartAfter = reason == IdleReason.JustStarted 
                ? TimeSpan.FromSeconds(1) 
                : TimeSpan.FromSeconds(60);
        }

        public IdleReason Reason { get; }
        public TimeSpan RestartAfter { get; }

        public static IdleData FromReason(IdleReason reason) => new(reason);
    }
}