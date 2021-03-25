using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery;
using CoreWars.Competition;
using CoreWars.Coordination.Messages;
using CoreWars.Player.Messages;

namespace CoreWars.Coordination
{
    public class CompetitionSlot
    {
        public 
    }
    public class CompetitionRoot : ReceiveActor
    {
        private readonly ICompetitionActorPropsFactory _competitionActorPropsFactory;
        private readonly IActorRef _competitionResultHandler;
        private readonly IActorRef _competitorAgentsSource;
        private readonly int _maxGamesAllowed;
        
        
        
        
        public CompetitionRoot(
            ICompetitionActorPropsFactory competitionActorPropsFactory
            , IActorRef competitionResultHandler
            , IActorRef competitorAgentsSource
            , int maxGamesAllowed = 100)
        {
            _competitionActorPropsFactory = competitionActorPropsFactory;
            _competitionResultHandler = competitionResultHandler;
            _competitorAgentsSource = competitorAgentsSource;
            _maxGamesAllowed = maxGamesAllowed;

            Receive<FillLobby>(OnFillLobby);
            Receive<CompetitionResultMessage>(OnGameConcluded);
            Receive<TypedQueryResult<AgentCreated>>(OnAgentsCreated);
            
            Receive<LobbyGameTerminated>((_) => GameCount--);
        }
        

        private int GameCount { get; set; } = 0;
        
        private void OnGameConcluded(CompetitionResultMessage obj)
        {
            _competitionResultHandler.Tell(obj);
            Sender.Tell(PoisonPill.Instance);
        }

        private void OnFillLobby(FillLobby obj)
        {
            var createGamesCount = _ - GameCount;

            for (var i = 0; i < createGamesCount; i++)
                Self.Tell(new OrderAgents());
        }
        
        private void OnAgentsCreated(TypedQueryResult<AgentCreated> obj)
        {
            var competitionAgents = obj.Result.Values.Select(x => x.AgentReference);
            var competitionProps = _competitionActorPropsFactory.Build(competitionAgents);
            var competitionActor = Context.ActorOf(competitionProps);

            Context.WatchWith(competitionActor, new LobbyGameTerminated(competitionActor));
            
            competitionActor.Tell(new Competition.Messages.RunCompetitionMessage());
            GameCount++;
        }
    }
}