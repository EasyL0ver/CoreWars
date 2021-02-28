using Autofac;
using BotArena.Common;
using BotArena.Modularity;

namespace PrisonerDilemma
{
    public class DilemmaModule : CompetitionModule
    {
        //strictly for 2 players
        public override Range<int> PlayersAllowedCount => Range<int>.Between(2, 2);
        
        public override void RegisterApi()
        {
            throw new System.NotImplementedException();
        }
    }
}