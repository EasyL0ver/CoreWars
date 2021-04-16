using System;
using System.Collections.Concurrent;
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
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Player;
using CoreWars.Scripting;
using CoreWars.WebApp.Mock;
using Microsoft.Extensions.Hosting;

namespace CoreWars.WebApp
{
    public sealed class AkkaGameService : IHostedService, IActorSystemService, IGameService
    {
        private readonly ConcurrentDictionary<Competition.ICompetition, IActorRef> _competitionLobbies;
        private readonly ILifetimeScope _container;

        public AkkaGameService(IServiceProvider serviceProvider)
        {
            _container = serviceProvider.GetAutofacRoot();
            _competitionLobbies = new ConcurrentDictionary<Competition.ICompetition, IActorRef>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var actorSystemConfig = HoconLoader.FromFile("./akka.hocon");
            ActorSystem = ActorSystem.Create("core-wars", actorSystemConfig);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ActorSystem.Dispose();
            
            return Task.CompletedTask;
        }

        public ActorSystem ActorSystem { get; private set; }
        public IReadOnlyList<Competition.ICompetition> AvailableCompetitions => _competitionLobbies.Keys.ToList();

        public void AddCompetitor(Props factory, Competition.ICompetition competition)
        {
            var playerLobby = _competitionLobbies[competition];
            ActorSystem.ActorOf(Competitor.Props(factory, playerLobby));
        }

        public void AddCompetition(Competition.ICompetition competition)
        {
            var resultHandler = ActorSystem.ActorOf<DummyCompetitionResultHandler>();
            var playerSet = _container.Resolve<ISelectableSet<IActorRef>>();
            var lobby = ActorSystem.ActorOf(PlayerLobby.Props(playerSet, competition.Info));
            
            for(var i = 0; i < competition.Info.MaxInstancesCount; i++)
            {
                var props = CompetitionSlot.Props(lobby, resultHandler, competition.Factory);
                var competitionSlot = ActorSystem.ActorOf(props);
                
                competitionSlot.Tell(RunCompetition.Instance);
            }

            _competitionLobbies[competition] = lobby;
        }

 
    }
}