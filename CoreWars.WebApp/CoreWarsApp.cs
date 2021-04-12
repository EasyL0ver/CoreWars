using Akka.Actor;
using CoreWars.App.Mock;
using CoreWars.Common;
using CoreWars.Competition;
using CoreWars.Coordination;
using CoreWars.Coordination.GameSlot;
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Player;
using CoreWars.Player.Messages;

namespace CoreWars.App
{
    public class CoreWarsApp
    {
        private readonly ActorSystem _actorSystem;

        public CoreWarsApp()
        {
            var actorSystemConfig = HoconLoader.FromFile("./akka.hocon");
            _actorSystem = ActorSystem.Create("core-wars", actorSystemConfig);
        }

        public void Initialize()
        {
            var randomLobbyStrategy = new SelectMaxRandomCollectionSelectionStrategy<IActorRef>();
            var config = new DummyCompetitionConfig();
            //todo replace with autofac modules
            var lobby = SetUpCompetitionModule(new RandomCompetitorWinsCompetitionFactory(), config, randomLobbyStrategy);
            
            AddMockPlayers(lobby, 5);
            
        }

        private void AddMockPlayers(IActorRef lobby, int amount)
        {
            var dummyCompetitorsFactory = new DummyCompetitorFactory();
            for (var i = 0; i < amount; i++)
            {
                _actorSystem.ActorOf(Competitor.Props(dummyCompetitorsFactory, lobby), "mock-player" + i);
            }
        }

        private IActorRef SetUpCompetitionModule(
            ICompetitionActorFactory competitionFactory
            , ILobbyConfig lobbyConfiguration
            , ICollectionSelectionStrategy<IActorRef> selectPlayersStrategy)
        {
            var resultHandler = _actorSystem.ActorOf<DummyCompetitionResultHandler>();
            var playerSet = new PlayerSet<IActorRef>(selectPlayersStrategy);
            var lobby = _actorSystem.ActorOf(PlayerLobby.Props(playerSet, lobbyConfiguration));
            
            for(var i = 0; i < lobbyConfiguration.MaxInstancesCount; i++)
            {
                var props = CompetitionSlot.Props(lobby, resultHandler, competitionFactory);
                var competitionSlot = _actorSystem.ActorOf(props);
                
                competitionSlot.Tell(RunCompetition.Instance);
            }

            return lobby;
        }
    
    }
}