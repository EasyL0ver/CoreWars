using CoreWars.Data.Entities;

namespace CoreWars.Player.Messages
{
    public class CompetitorScriptUpdated
    {
        public CompetitorScriptUpdated(Script newScript)
        {
            NewScript = newScript;
        }

        public Script NewScript { get; }
       
    }
}