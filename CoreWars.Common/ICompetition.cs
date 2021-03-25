namespace CoreWars.Common
{
    public interface ICompetition
    {
        public string Name { get; }
        public Range<int> PlayerCount { get; }
    }
}