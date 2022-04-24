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


//// Quick test, should result in 0.056
//List<BinanceTrade> result = new List<BinanceTrade>();
//result.Add(new BinanceTrade()
//{
//    Timestamp = DateTime.Now,
//    Symbol = "ALGOUSDT",
//    IsBuyer = true,
//    Quantity = 11,
//    Price = 0.9272m,
//    QuoteQuantity = 10.1992m
//});
//result.Add(new BinanceTrade()
//{
//    Timestamp = DateTime.Now.AddHours(result.Count()),
//    Symbol = "ALGOUSDT",
//    IsBuyer = true,
//    Quantity = 26,
//    Price = 0.9244m,
//    QuoteQuantity = 24.0344m
//});
//result.Add(new BinanceTrade()
//{
//    Timestamp = DateTime.Now.AddHours(result.Count()),
//    Symbol = "ALGOUSDT",
//    IsBuyer = false,
//    Quantity = 15,
//    Price = 0.929m,
//    QuoteQuantity = 13.935m
//});