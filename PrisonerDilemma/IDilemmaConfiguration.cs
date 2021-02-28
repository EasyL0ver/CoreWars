using System;

namespace PrisonerDilemma
{
    public interface IDilemmaConfiguration
    {
        public int BothCooperateScore { get; }
        public int BothDefectScore { get; }
        public int DefectScore { get; }
        public int CooperateScore { get; }
        public int IterationsCount { get; }
        public TimeSpan ResponseTimeout { get; } 
    }

    public class DefaultDilemmaConfiguration : IDilemmaConfiguration
    {
        public int BothCooperateScore { get; } = 10;
        public int BothDefectScore { get; } = 0;
        public int DefectScore { get; } = 15;
        public int CooperateScore { get; } = 3;
        public int IterationsCount { get; } = 100;
        public TimeSpan ResponseTimeout { get; } = TimeSpan.FromSeconds(30);
    }
}