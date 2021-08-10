using System;

namespace PrisonerDilemma
{
    public class DilemmaConfiguration : IDilemmaConfiguration
    {
        public DilemmaConfiguration(int cooperateScore, int defectScore, int bothDefectScore, int bothCooperateScore, int iterationsCount, TimeSpan timeout)
        {
            CooperateScore = cooperateScore;
            DefectScore = defectScore;
            BothDefectScore = bothDefectScore;
            BothCooperateScore = bothCooperateScore;
            IterationsCount = iterationsCount;
            Timeout = timeout;
        }

        public static IDilemmaConfiguration Default()
        {
            return new DilemmaConfiguration(0, 3, 1, 2, 100, TimeSpan.FromSeconds(5));
        }

        public int IterationsCount { get; }
        public TimeSpan Timeout { get; }
        public int BothCooperateScore { get; }
        public int BothDefectScore { get; }
        public int CooperateScore { get; }
        public int DefectScore { get; }
        
        
    }
}