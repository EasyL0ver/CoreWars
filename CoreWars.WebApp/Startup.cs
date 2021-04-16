using System.Collections.Generic;
using Akka.Actor;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreWars.Common;
using CoreWars.Competition;
using CoreWars.Coordination;
using CoreWars.Coordination.GameSlot;
using CoreWars.Coordination.Messages;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Scripting.Python;
using CoreWars.WebApp.Controllers;
using CoreWars.WebApp.Mock;
using DummyCompetition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ICompetition = CoreWars.Competition.ICompetition;

namespace CoreWars.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreWars.WebApp", Version = "v1" });
            });
            services.AddHostedService(x => (AkkaGameService) x.GetService<IActorSystemService>());
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        // ReSharper disable once UnusedMember.Global
        public void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterType<AkkaGameService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<SelectMaxRandomCollectionSelectionStrategy<IActorRef>>()
                .AsImplementedInterfaces();

            var randomLobbyStrategy = new SelectMaxRandomCollectionSelectionStrategy<IActorRef>();
            //todo replace with autofac modules
            //var lobby = SetUpCompetitionModule(new RandomCompetitorWinsCompetitionPropsFactory(), config, randomLobbyStrategy);

            //builder.RegisterType<DiTest3>();
            //builder.RegisterType<SampleAddCompetitorController>();
            //builder.RegisterInstance(lobby);
            builder.RegisterModule<PythonScriptingModule>();
            builder.RegisterModule<DummyCompetitionModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeGameServer(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreWars.WebApp v1"));
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void InitializeGameServer(IApplicationBuilder app)
        {
            var container = app.ApplicationServices.GetAutofacRoot();
            var actorSystem = app.ApplicationServices.GetService<IGameService>();

            container
                .Resolve<IEnumerable<ICompetition>>()
                .ForEach(c => actorSystem.AddCompetition(c));
        }
        
        // private ILobby SetUpCompetitionModule(
        //     ICompetitionActorPropsFactory competitionPropsFactory
        //     , ILobbyConfig lobbyConfiguration
        //     , ICollectionSelectionStrategy<IActorRef> selectPlayersStrategy)
        // {
        //     var resultHandler = ActorSystem.ActorOf<DummyCompetitionResultHandler>();
        //     var playerSet = new PlayerSet<IActorRef>(selectPlayersStrategy);
        //     var lobby = ActorSystem.ActorOf(PlayerLobby.Props(playerSet, lobbyConfiguration));
        //     
        //     for(var i = 0; i < lobbyConfiguration.MaxInstancesCount; i++)
        //     {
        //         var props = CompetitionSlot.Props(lobby, resultHandler, competitionPropsFactory);
        //         var competitionSlot = ActorSystem.ActorOf(props);
        //         
        //         competitionSlot.Tell(RunCompetition.Instance);
        //     }
        //
        //     return new Lobby(lobby);
        // }
    }
}
