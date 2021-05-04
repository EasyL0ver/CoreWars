using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreWars.Common;
using CoreWars.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreWars.WebApp
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            InitializeDatabase(host);
            
            host.Run();
        }

        
        private static void InitializeDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var container = scope.ServiceProvider.GetAutofacRoot();
            var context = container.Resolve<CoreWarsDataContext>();

            context.Database.EnsureCreated();
            
            var competitions = container.Resolve<IEnumerable<ICompetition>>();
            context.SeedCompetitionInfo(competitions);

            var scriptingLanguages = container.Resolve<IEnumerable<ICompetitorFactory>>();
            context.SeedLanguageInfo(scriptingLanguages.SelectMany(x => x.SupportedCompetitionNames));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}