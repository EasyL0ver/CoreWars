using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions;
using CoreWars.Coordination;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Data;
using CoreWars.Game;
using CoreWars.Player;
using CoreWars.Player.Messages;
using CoreWars.Scripting;
using CoreWars.WebApp.Actors;
using CoreWars.WebApp.Actors.Notification;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Messages = CoreWars.WebApp.Actors.Messages;

namespace CoreWars.WebApp
{
    public sealed class AkkaGameService : IHostedService, IGameService
    {
        private readonly ILifetimeScope _container;
        
        public AkkaGameService(IServiceProvider serviceProvider)
        {
            _container = serviceProvider.GetAutofacRoot();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var actorSystemConfig = HoconLoader.FromFile("./akka.hocon");
            ActorSystem = ActorSystem.Create("core-wars", actorSystemConfig);

            var scriptRepositoryProps = Data.ScriptRepository.Props(_container.Resolve<IDataContext>());
            ScriptRepository = ActorSystem.ActorOf(scriptRepositoryProps);
            

            
            var statsRepositoryProps = Props.Create(() => new StatsRepository(_container.Resolve<IDataContext>()));
            ResultsHandler = ActorSystem.ActorOf(statsRepositoryProps);

            RegisterNotifications();
            
            
            CompetitorFactory = _container.Resolve<AggregatedCompetitorFactory>();
            var competitorRootProps =
                Props.Create(() => new CompetitorsRoot(ScriptRepository, CompetitorFactory, ResultsHandler));
            CompetitorsRoot = ActorSystem.ActorOf(competitorRootProps, "competitors");

            InitializeCompetitions();

            return Task.CompletedTask;
        }

        private void InitializeCompetitions()
        {
            _container
                .Resolve<IEnumerable<ICompetitionRegistration>>()
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
        public IActorRef NotificationProvider { get; private set; }
        public ICompetitorFactory CompetitorFactory { get; private set; }
        public IActorRef CompetitorsRoot { get; private set; }
        
        
        

        private void AddCompetition(ICompetitionRegistration competitionRegistration)
        {
            var playerSet = _container.Resolve<ISelectableSet<IActorRef>>();
            var lobby = ActorSystem.ActorOf(CompetitorLobby.Props(playerSet, competitionRegistration.Info));
          
            for(var i = 0; i < competitionRegistration.Info.MaxInstancesCount; i++)
            {
                var props = CompetitionSlot.Props(lobby, competitionRegistration.Factory);
                ActorSystem.ActorOf(props, $"{competitionRegistration.Info.Name}-slot-{i}");
            }
            
            CompetitorsRoot.Tell(new AddCompetition(lobby, competitionRegistration.Info));
        }

        private void RegisterNotifications()
        {
            var hubContext = _container.Resolve<IHubContext<CompetitorNotificationHub>>();
            Func<Messages.RegisterCompetitorNotifications, Props> factory =
                (msg)=>
                {
                    return Props.Create(() => new StatusObserver(msg.CompetitorId, hubContext, msg.NotificationId));

                };
            var notifierProps = Props.Create(() => new NotificationRoot(factory));
            NotificationProvider = ActorSystem.ActorOf(notifierProps);
        }

 
    }
}