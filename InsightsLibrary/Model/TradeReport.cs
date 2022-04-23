using Binance.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Model
{
    public class TradeReport
    {
        public readonly string symbol;
        public DateTime Key { get; }


        public decimal realizedPNL;
        public decimal totalPNL;

        public List<BinanceTrade> trades = new List<BinanceTrade>();
        public Position position;
        public decimal accumlatedPositionValueDelta;

        public int totalBuys, totalSells;
        public decimal totalBuyQuantity, totalSellQuantity;
        public decimal totalBuyQuantityValue, totalSellQuantityValue;

        public TradeReport(DateTime key, string symbol, Position? inheritedPosition = null)
        {
            Key = key;
            this.symbol = symbol;
            position = inheritedPosition ?? new Position();
        }
    }

    /// <summary>
    /// (Buying price * quantity bought) - (Selling price * quantity sold)
    /// </summary>
    public struct Position
    {
        public decimal AccumulatedQuantity { get; private set; }
        public decimal PositionPrice { get; private set; }

        public decimal PositionValue
        {
            get { return AccumulatedQuantity * PositionPrice; }
        }

        public void AddEntry(BinanceTrade trade)
        {
            if (trade.IsBuyer)
            {
                SetPositionPrice(trade);
                AccumulatedQuantity += trade.Quantity;
            }
            else
            {
                AccumulatedQuantity -= trade.Quantity;
            }
        }

        private void SetPositionPrice(BinanceTrade trade)
        {
            if (PositionPrice == 0)
                PositionPrice = trade.Price;
            else
            {
                var totalValue = PositionValue + trade.QuoteQuantity;
                PositionPrice = totalValue / (AccumulatedQuantity + trade.Quantity);
            }
        }

        public bool IsOpen()
        {
            return AccumulatedQuantity > 0;
        }
    }
}

