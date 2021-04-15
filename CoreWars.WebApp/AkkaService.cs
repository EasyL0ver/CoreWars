using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using Microsoft.Extensions.Hosting;

namespace CoreWars.WebApp
{
    public sealed class AkkaService : IHostedService, IActorSystemService
    {
        private readonly IServiceProvider _serviceProvider;

        public AkkaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var actorSystemConfig = HoconLoader.FromFile("./akka.hocon");
            ActorSystem = ActorSystem.Create("core-wars", actorSystemConfig);

            var test = _serviceProvider.GetService(typeof(DiTest3));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ActorSystem.Dispose();
            
            return Task.CompletedTask;
        }

        public ActorSystem ActorSystem { get; private set; }
    }
}