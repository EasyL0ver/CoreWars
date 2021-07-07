namespace CoreWars.Common
{
    public interface ICompetition
    {
        ICompetitionInfo Info { get; }
        ICompetitionActorPropsFactory Factory { get; }
    }
}