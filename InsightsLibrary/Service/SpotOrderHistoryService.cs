using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using InsightsLibrary.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightsLibrary.Service
{
    public interface ISpotOrderHistoryService
    {
        Task<TradeReportResult> GetTradeReports(string symbol);
    }

    public class SpotOrderHistoryService : BinanceService, ISpotOrderHistoryService
    {
        public SpotOrderHistoryService(IBinanceClient binanceClient, ILogger<SpotOrderHistoryService> logger) : base(binanceClient, logger)
        {

        }

        public async Task<TradeReportResult> GetTradeReports(string symbol)
        {
            var userTradesResult = await binanceClient.SpotApi.Trading.GetUserTradesAsync(symbol);
            userTradesResult.GetResultOrError(out IEnumerable<BinanceTrade> result, out Error error);

            // Loop all trades per day, any quantities left over at end of day are passed on to the next day
            // Current/last day: ignore quantities bought which have not yet been sold

            var groups = result.GroupBy(key =>
                new DateTime(year: key.Timestamp.Year, month: key.Timestamp.Month, day: key.Timestamp.Day));

            List<TradeReport> reports = new List<TradeReport>();

            var factory = new TradeReportFactory();
            foreach (var group in groups)
            {
                var report = await factory.Create(group.Key.Date, group.ToList(), symbol);
                reports.Add(report);
            }

            return new TradeReportResult(reports);
        }

    }
}
