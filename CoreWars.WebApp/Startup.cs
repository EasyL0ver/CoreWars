using Akka.Actor;
using Autofac;
using CoreWars.Coordination.PlayerSet;
using CoreWars.Data;
using CoreWars.Scripting.Python;
using DummyCompetition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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

            //services.AddDbContext<IBaseRepository, CoreWarsDataContext>();
            
            services.AddHostedService(x => (AkkaGameService) x.GetService<IGameService>());
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
                .As<IGameService>()
                .SingleInstance();


            var connectionString = Configuration.GetSection("ConnectionString").Value;

            builder.RegisterType<CoreWarsDataContext>()
                .WithParameter("connectionString", connectionString)
                .As<CoreWarsDataContext>()
                .AsImplementedInterfaces();

            builder.RegisterType<AggregatedCompetitorFactory>();

            builder
                .RegisterType<SelectMaxRandomCollectionSelectionStrategy<IActorRef>>()
                .AsImplementedInterfaces();

            builder.RegisterType<PlayerSet<IActorRef>>()
                .AsImplementedInterfaces();

            builder.RegisterModule<PythonScriptingModule>();
            builder.RegisterModule<DummyCompetitionModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
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

    }
}
