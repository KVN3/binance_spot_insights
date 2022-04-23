using Binance.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsightsLibrary.Model
{
    public class TradeReportFactory
    {
        private TradeReport tradeReport;

        // From previous/current day
        private Position inheritedPosition;

        public TradeReportFactory()
        {
            
        }

        public async Task<TradeReport> Create(DateTime key, IEnumerable<BinanceTrade> trades, string symbol)
        {
            this.tradeReport = new TradeReport(key, symbol, inheritedPosition);

            foreach (var trade in trades)
                AppendTrade(trade);

            tradeReport.realizedPNL = tradeReport.accumlatedPositionValueDelta;

            // Position left open at the end of the day
            if (tradeReport.position.IsOpen())
            {
                // Total PNL requires current price
                //tradeReport.totalPNL = tradeReport.accumlatedPositionValueDelta - tradeReport.position.PositionValue;

                // Account for open position
                tradeReport.realizedPNL -= tradeReport.position.PositionValue;

            }

            tradeReport.realizedPNL *= -1;
            inheritedPosition = tradeReport.position;

            return tradeReport;
        }

        private void AppendTrade(BinanceTrade trade)
        {
            tradeReport.trades.Add(trade);
            tradeReport.position.AddEntry(trade);

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
