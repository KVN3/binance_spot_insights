using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsightsLibrary.Service
{
    public interface ISpotOrderHistoryService
    {

    }

    public class SpotOrderHistoryService : BinanceService, ISpotOrderHistoryService
    {
        public SpotOrderHistoryService(IBinanceClient binanceClient, ILogger<SpotOrderHistoryService> logger) : base(binanceClient, logger)
        {

        }

        public async Task<IEnumerable<BinanceTrade>> GetRealizedPNL()
        {
            var userTradesResult = await binanceClient.SpotApi.Trading.GetUserTradesAsync("GMTUSDT");
            userTradesResult.GetResultOrError(out IEnumerable<BinanceTrade> result, out Error error);

            return result;
        }

    }
}
