namespace CoreWars.Coordination
{
    public class Uninitialized : ICompetitionSlotFSMData
    {
        public static Uninitialized Instance => new Uninitialized();
    }
}