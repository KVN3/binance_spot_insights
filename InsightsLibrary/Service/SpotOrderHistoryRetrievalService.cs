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
    public interface ISpotOrderHistoryRetrievalService
    {
        Task<HashSet<BinanceTradeWrapper>> GetUserTradesForRange(string symbol, TimeRange range);
    }

    public class SpotOrderHistoryRetrievalService : BinanceService, ISpotOrderHistoryRetrievalService
    {
        private const int MAX_HOURS_PER_PERIOD = 24;
        private const int MAX_TRADES_PER_CALL = 500;

        // Request specific
        private HashSet<BinanceTradeWrapper> userTrades;
        private string symbol;
        private TimeRange range;

        public SpotOrderHistoryRetrievalService(IBinanceClient binanceClient, ILogger<SpotOrderHistoryRetrievalService> logger)
            : base(binanceClient, logger)
        {

        }

        public async Task<HashSet<BinanceTradeWrapper>> GetUserTradesForRange(string symbol, TimeRange range)
        {
            Reset();

            // General request data
            this.symbol = symbol;
            this.range = range;

            // Data
            var firstPeriod = new OrderPeriod(range);
            await RetrieveTrades(firstPeriod);
            CheckForDuplicates();


            return userTrades;
        }

        private void CheckForDuplicates()
        {
            List<long> ids = new List<long>();

            foreach (var item in userTrades)
            {
                if (ids.Contains(item.trade.Id))
                {
                    var a = "";
                }
                else
                {

                    ids.Add(item.trade.Id);
                }

            }
        }

        private void Reset()
        {
            userTrades = userTrades = new HashSet<BinanceTradeWrapper>();
        }

        private async Task RetrieveTrades(OrderPeriod period)
        {
            if (period.isEndReached)
                return;

            // Get results
            var userTradesResult = await binanceClient.SpotApi.Trading.GetUserTradesAsync(symbol, startTime: period.iterationStart, endTime: period.iterationEnd);
            if (!userTradesResult.GetResultOrError(out IEnumerable<BinanceTrade> result, out Error error))
                throw new Exception(error.Message);

            // Parse them
            if (result != null)
            {
                var newTrades = result.ToList();

                foreach (var trade in newTrades)
                    userTrades.Add(new BinanceTradeWrapper(trade));

                await RetrieveTrades(period.GoNext(newTrades));
            }

            return;
        }

        private struct OrderPeriod
        {
            private readonly TimeRange range;
            public DateTime iterationStart;
            public DateTime iterationEnd;

            public bool isEndReached;

            //public bool IsFullyTraversed(int lastTradeCount)
            //{
            //    if (lastTradeCount == MAX_TRADES_PER_CALL)
            //        return false;

            //    return isEndReached;
            //}

            public OrderPeriod(TimeRange range)
            {
                this.range = range;
                this.iterationStart = (DateTime)range.start;
                this.iterationEnd = DateTime.MinValue;
                this.isEndReached = false;
                SetIterationEnd(0);
            }

            public OrderPeriod GoNext(List<BinanceTrade> newTrades)
            {
                if (IsResultCapReached(newTrades.Count))
                    this.iterationStart = newTrades[newTrades.Count() - 1].Timestamp;
                else
                    this.iterationStart = iterationEnd;

                SetIterationEnd(newTrades.Count);

                return this;
            }

            private bool IsResultCapReached(int tradeCount)
            {
                return tradeCount == MAX_TRADES_PER_CALL;
            }

            private void SetIterationEnd(int tradeCount)
            {
                this.iterationEnd = this.iterationStart.AddHours(MAX_HOURS_PER_PERIOD);

                // Reached the end
                if (this.iterationEnd > this.range.end)
                {
                    this.iterationEnd = (DateTime)this.range.end;

                    if (!IsResultCapReached(tradeCount))
                        this.isEndReached = true;
                }
            }
        }
    }
}



//// First trade found, we stop looping
//if (userTrades.Count > 0 && firstTradeInRange.Id == userTrades[0].Id)
//    return;

// We do this to avoid missing trades at the same time and the end of the range:
// Limit 500, there's 2 trades at 16:00:00, but the second trade is the 501th trade
// So we can't just decrease the endTime by a second, as we would miss the 2nd trade at 16:00
// Trash solution is to pass the same timestamp as endTime and filter double trades out at the end of the range
// Can still miss a trade if limit is met at the same stamp
//var indexesToRemove = new List<int>();

//for (int x = 0; x < newTrades.Count(); x++)
//{
//    var newTrade = newTrades[x];
//    bool isDuplicate = false;


//    for (int i = userTrades.Count() - 1; i >= 0; i--)
//    {
//        var existingTrade = userTrades[i];

//        if (newTrade.Id == existingTrade.Id)
//        {
//            isDuplicate = true;
//            indexesToRemove.Add(x);
//        }
//        else
//        {
//            break;
//        }
//    }

//    // Need only check the rear
//    if (!isDuplicate)
//        break;
//}

//foreach (int x in indexesToRemove)
//{
//    newTrades.RemoveAt(x);
//}