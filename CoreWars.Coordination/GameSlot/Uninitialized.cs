namespace CoreWars.Coordination.GameSlot
{
    public class Uninitialized : ICompetitionSlotFSMData
    {
        public static Uninitialized Instance => new Uninitialized();
    }
}