using DataRepository;
using Microsoft.Extensions.DependencyInjection;

namespace InsightsLibrary.Infrastructure
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddDataLibrary(
            this IServiceCollection services)
        {
            services.AddDatabaseLibrary();
            services.AddLogicLibrary();

            return services;
        }

        private static IServiceCollection AddDatabaseLibrary(this IServiceCollection services)
        {
            services.AddSingleton(new ConnectivityTest());

            return services;
        }


        private static IServiceCollection AddLogicLibrary(
            this IServiceCollection services)
        {

            return services;
        }
    }
}