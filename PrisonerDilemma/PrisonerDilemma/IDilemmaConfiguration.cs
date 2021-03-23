using System;

namespace PrisonerDilemma
{
    public interface IDilemmaConfiguration
    {
        int IterationsCount { get; }
        TimeSpan Timeout { get; }
        int BothCooperateScore { get; }
        int BothDefectScore { get; }
        int CooperateScore { get; }
        int DefectScore { get; }
    }
}