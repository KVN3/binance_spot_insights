﻿using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using InsightsLibrary.Service;
using Microsoft.Extensions.DependencyInjection;

namespace InsightsLibrary.Infrastructure
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddLibrary(
            this IServiceCollection services,
            Configuration.ApiCredentials apiCredentials)
        {
            //services.AddLogging
            services.AddDataLibrary(apiCredentials);
            services.AddLogicLibrary();

            return services;
        }

        private static IServiceCollection AddDataLibrary(
            this IServiceCollection services,
            Configuration.ApiCredentials apiCredentials)
        {
            IBinanceClient binanceClient = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(apiCredentials.key, apiCredentials.secret)
            });

            services.AddSingleton(binanceClient);

            services.AddTransient<ISpotOrderHistoryService, SpotOrderHistoryService>();

            return services;
        }


        private static IServiceCollection AddLogicLibrary(
            this IServiceCollection services)
        {

            return services;
        }
    }
}