using Akka.Actor;
using CoreWars.Competition;

namespace CoreWars.Coordination.GameSlot
{
    public class ConcludedGameData : ICompetitionSlotFSMData
    {
        public ConcludedGameData(
            IActorRef game
            , CompetitionResultMessage result)
        {
            Game = game;
            Result = result;
            ResultAcknowledged = false;
            GameTerminated = false;
        }

        private ConcludedGameData(IActorRef game, CompetitionResultMessage message, bool resultAcknowledged = false,
            bool gameTerminated = false) : this(game, message)
        {
            ResultAcknowledged = resultAcknowledged;
            GameTerminated = gameTerminated;
        }

        public IActorRef Game { get; }
        public CompetitionResultMessage Result { get; }
        public bool ResultAcknowledged { get; }
        public bool GameTerminated { get; }
        public bool FullyConcluded => ResultAcknowledged && GameTerminated;

        public ConcludedGameData WithResultAcknowledged => new ConcludedGameData(Game, Result, resultAcknowledged: true,
            gameTerminated: GameTerminated);
        
        public ConcludedGameData WithGameTerminated => new ConcludedGameData(Game, Result, resultAcknowledged: ResultAcknowledged,
            gameTerminated: true);
        
        
    }
}