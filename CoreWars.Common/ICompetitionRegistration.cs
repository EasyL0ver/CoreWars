namespace CoreWars.Common
{
    public interface ICompetitionRegistration
    {
        ICompetitionInfo Info { get; }
        ICompetitionActorPropsFactory Factory { get; }
    }
}