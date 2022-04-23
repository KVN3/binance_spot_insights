using Binance.Net.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Service
{
    public class BinanceService
    {
        protected readonly IBinanceClient binanceClient;
        protected readonly ILogger logger;

        public BinanceService(IBinanceClient binanceClient, ILogger logger)
        {
            this.binanceClient = binanceClient;
            this.logger = logger;
        }
    }
}
