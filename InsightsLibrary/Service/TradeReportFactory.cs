using Binance.Net.Objects.Models.Spot;
using InsightsLibrary.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsightsLibrary.Model
{
    public interface ITradeReportFactory
    {
        Task<TradeReport> Create(DateTime key, IEnumerable<BinanceTrade> trades, string symbol);
    }

    public class TradeReportFactory : ITradeReportFactory
    {
        private readonly IBookService bookService;
        private TradeReport tradeReport;

        // From previous/current day
        private Position inheritedPosition;
        private decimal inheritedValue;

        public TradeReportFactory(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task<TradeReport> Create(DateTime key, IEnumerable<BinanceTrade> trades, string symbol)
        {
            this.tradeReport = new TradeReport(key, symbol, inheritedPosition);
            this.inheritedValue = inheritedPosition.PositionValue;

            foreach (var trade in trades)
                AppendTrade(trade);

            tradeReport.CalculateRealizedPNL();

            tradeReport.totalFeeCost = await new FeeSummaryService(tradeReport, bookService).SummarizeFeeCost();

            inheritedPosition = tradeReport.position;

            return tradeReport;
        }

        private void AppendTrade(BinanceTrade trade)
        {
            tradeReport.trades.Add(trade);
            tradeReport.position.AddEntry(trade);

            tradeReport.AddFeeEntry(trade.FeeAsset, trade.Fee);

            if (trade.IsBuyer)
            {
                tradeReport.totalBuyQuantity += trade.Quantity;
                tradeReport.totalBuyQuantityValue += trade.QuoteQuantity;
                tradeReport.accumlatedPositionValueDelta += trade.QuoteQuantity;
                tradeReport.totalBuys += 1;
            }
            else
            {
                tradeReport.totalSellQuantity += trade.Quantity;
                tradeReport.totalSellQuantityValue += trade.QuoteQuantity;
                tradeReport.accumlatedPositionValueDelta -= trade.QuoteQuantity;
                tradeReport.totalSells += 1;
            }
        }




    }
}
