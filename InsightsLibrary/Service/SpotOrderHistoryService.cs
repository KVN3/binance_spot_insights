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
        private List<BinanceTrade> userTrades;
        private readonly ITradeReportFactory factory;
        private readonly IBookService bookService;

        public SpotOrderHistoryService(IBinanceClient binanceClient, ILogger<SpotOrderHistoryService> logger,
            ITradeReportFactory factory, IBookService bookService) : base(binanceClient, logger)
        {
            this.factory = factory;
            this.bookService = bookService;
        }

        public async Task<TradeReportResult> GetTradeReports(string symbol)
        {
            await GetAllUserTrades(symbol);

            var groups = userTrades.GroupBy(key =>
                new DateTime(year: key.Timestamp.Year, month: key.Timestamp.Month, day: key.Timestamp.Day));

            List<TradeReport> reports = new List<TradeReport>();

            foreach (var group in groups)
            {
                var report = await factory.Create(group.Key.Date, group.ToList(), symbol);
                reports.Add(report);
            }

            return new TradeReportResult(reports, bookService);
        }

        private async Task GetAllUserTrades(string symbol, DateTime? startTime = null, DateTime? endTime = null)
        {
            // Last 500 orders and start
            if (startTime == null && endTime == null)
                userTrades = new List<BinanceTrade>();

            var userTradesResult = await binanceClient.SpotApi.Trading.GetUserTradesAsync(symbol, startTime: startTime, endTime: endTime);
            userTradesResult.GetResultOrError(out IEnumerable<BinanceTrade> result, out Error error);

            if (result != null && result.Count() > 0)
            {
                var newRange = result.ToList();
                var firstTradeInRange = newRange[0];

                // First trade found, we stop looping
                if (userTrades.Count > 0 && firstTradeInRange.Id == userTrades[0].Id)
                    return;

                // We do this to avoid missing trades at the same time and the end of the range:
                // Limit 500, there's 2 trades at 16:00:00, but the second trade is the 501th trade
                // So we can't just decrease the endTime by a second, as we would miss the 2nd trade at 16:00
                // Trash solution is to pass the same timestamp as endTime and filter double trades out at the end of the range
                // Can still miss a trade if limit is met at the same stamp
                var indexesToRemove = new List<int>();

                for (int i = newRange.Count() - 1; i >= 0; i--)
                {
                    bool isDuplicate = false;

                    for (int x = 0; x < userTrades.Count(); x++)
                    {
                        if (newRange[i].Id == userTrades[x].Id)
                        {
                            isDuplicate = true;
                            indexesToRemove.Add(i);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Need only check the rear
                    if (!isDuplicate)
                        break;
                }

                foreach (int i in indexesToRemove)
                {
                    newRange.RemoveAt(i);
                }

                userTrades.InsertRange(0, newRange);

                // Next iter
                startTime = firstTradeInRange.Timestamp.AddDays(-1);
                endTime = firstTradeInRange.Timestamp;
                await GetAllUserTrades(symbol, startTime, endTime);
            }

            return;
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