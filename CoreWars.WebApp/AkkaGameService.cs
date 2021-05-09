using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreWars.Common;
using CoreWars.Coordination;
using CoreWars.Coordination.GameSlot;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Data;
using CoreWars.Data.Entities;
using CoreWars.Player;
using Microsoft.Extensions.Hosting;
using ICompetition = CoreWars.Common.ICompetition;

namespace CoreWars.WebApp
{
    public sealed class AkkaGameService : IHostedService, IGameService
    {
        private readonly ILifetimeScope _container;
        private readonly List<ICompetitionInfo> _supportedCompetitions;
        
        public AkkaGameService(IServiceProvider serviceProvider)
        {
            _container = serviceProvider.GetAutofacRoot();
            _supportedCompetitions = new List<ICompetitionInfo>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var actorSystemConfig = HoconLoader.FromFile("./akka.hocon");
            ActorSystem = ActorSystem.Create("core-wars", actorSystemConfig);

            var data = _container.Resolve<IDataContext>();
            var scriptRepositoryProps = ScriptRepositoryActor.Props(data);
            ScriptRepository = ActorSystem.ActorOf(scriptRepositoryProps);

            var statsRepositoryProps = Props.Create(() => new StatsRepositoryActor(data));
            ResultsHandler = ActorSystem.ActorOf(statsRepositoryProps);


            InitializeCompetitions();

            return Task.CompletedTask;
        }

        private void InitializeCompetitions()
        {
            _container
                .Resolve<IEnumerable<ICompetition>>()
                .ForEach(AddCompetition);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ActorSystem.Dispose();
            return Task.CompletedTask;
        }

        public ActorSystem ActorSystem { get; private set; }
        public IActorRef ScriptRepository { get; set; }
        public IActorRef ResultsHandler { get; private set; }
        
        public IReadOnlyList<ICompetitionInfo> AvailableCompetitions => _supportedCompetitions.ToList();

   
        private void AddCompetition(ICompetition competition)
        {
            var playerSet = _container.Resolve<ISelectableSet<IActorRef>>();
            var competitorFactory = _container.Resolve<AggregatedCompetitorFactory>();
            var lobby = ActorSystem.ActorOf(PlayerLobby.Props(playerSet, competition.Info));

            var competitorsRootProps = CompetitorRoot.Props(ScriptRepository, competition.Info, lobby, competitorFactory);
            ActorSystem.ActorOf(competitorsRootProps);
            
            
            for(var i = 0; i < competition.Info.MaxInstancesCount; i++)
            {
                var props = CompetitionSlot.Props(lobby, ResultsHandler, competition.Factory);
                ActorSystem.ActorOf(props, $"{competition.Info.Name}-{i}");
            }

            _supportedCompetitions.Add(competition.Info);
        }

 
    }
}