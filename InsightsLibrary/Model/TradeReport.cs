using Binance.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Model
{
    public class TradeReport
    {
        public string Symbol { get; }
        public DateTime Key { get; }


        public decimal RealizedPNL { get; private set; }
        public decimal UnrealizedPNL { get; private set; }

        public List<BinanceTrade> trades = new List<BinanceTrade>();

        public Position initialPosition;
        public Position position;
        public decimal accumlatedPositionValueDelta;

        public int totalBuys, totalSells;
        public decimal totalBuyQuantity, totalSellQuantity;
        public decimal totalBuyQuantityValue, totalSellQuantityValue;

        public TradeReport(DateTime key, string symbol, Position? inheritedPosition = null)
        {
            Key = key;
            Symbol = symbol;

            initialPosition = inheritedPosition ?? new Position();
            position = inheritedPosition ?? new Position();
        }

        public void CalculateRealizedPNL()
        {
            RealizedPNL = -1 * (totalBuyQuantityValue + initialPosition.PositionValue - totalSellQuantityValue - position.PositionValue);
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

