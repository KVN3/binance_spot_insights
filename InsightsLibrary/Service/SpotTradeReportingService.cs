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
    public interface ISpotTradeReportingService
    {
        Task<TradeReportResult> GetTradeReports(string symbol, TimeRange timeRange);
    }

    public class SpotTradeReportingService : BinanceService, ISpotTradeReportingService
    {
        private readonly ITradeReportFactory factory;
        private readonly IBookService bookService;
        private readonly ISpotOrderHistoryRetrievalService retrievalService;

        public SpotTradeReportingService(IBinanceClient binanceClient, ILogger<SpotTradeReportingService> logger,
            ITradeReportFactory factory, IBookService bookService, ISpotOrderHistoryRetrievalService retrievalService) : base(binanceClient, logger)
        {
            this.factory = factory;
            this.bookService = bookService;
            this.retrievalService = retrievalService;
        }

        public async Task<TradeReportResult> GetTradeReports(string symbol, TimeRange timeRange)
        {
            HashSet<BinanceTradeWrapper> userTrades = await retrievalService.GetUserTradesForRange(symbol, timeRange);

            var groups = userTrades.GroupBy(key =>
                new DateTime(year: key.trade.Timestamp.Year, month: key.trade.Timestamp.Month, day: key.trade.Timestamp.Day));

            List<TradeReport> reports = new List<TradeReport>();

            foreach (var group in groups)
            {
                var report = await factory.Create(group.Key.Date, group.ToList(), symbol);
                reports.Add(report);
            }

            return new TradeReportResult(reports, bookService);
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