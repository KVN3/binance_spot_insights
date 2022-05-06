using InsightsLibrary.Infrastructure;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;


namespace ConsoleApp.Infrastructure
{
    public sealed class DIContainer
    {
        private static readonly IServiceProvider _instance = Build();
        public static IServiceProvider Instance => _instance;

        static DIContainer()
        {

        }

        private DIContainer()
        {

        }

        public static IServiceProvider Build()
        { 
           ServiceCollection services = new ServiceCollection();

            //Cloud uses environment variables, local uses local.settings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Settings settings = new Settings(configuration);
            services.AddSingleton(settings);
            services.AddLogging();

            services.AddInsightsLibrary(settings.ApiCredentials);
            services.AddDataLibrary();

            return services.BuildServiceProvider();
        }

    }
}
