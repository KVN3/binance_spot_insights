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

        // All fees and trades
        public List<BinanceTrade> trades = new List<BinanceTrade>();
        public Dictionary<string, decimal> fees = new Dictionary<string, decimal>();

        // Position
        public Position initialPosition;
        public Position position;
        public decimal accumlatedPositionValueDelta;

        // Totals
        public int totalBuys, totalSells;
        public decimal totalBuyQuantity, totalSellQuantity;
        public decimal totalBuyQuantityValue, totalSellQuantityValue;
        public decimal totalFeeCost;

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

        public void AddFeeEntry(string feeAsset, decimal fee)
        {
            if (!fees.ContainsKey(feeAsset))
                fees.Add(feeAsset, fee);
            else
                fees[feeAsset] += fee;
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
            //if (AccumulatedQuantity == 200000 || AccumulatedQuantity + trade.Quantity == 200000 || AccumulatedQuantity - trade.Quantity == 200000)
            //{
            //    var a = "";
            //}

            if (trade.IsBuyer)
            {
                SetPositionPrice(trade);
                AccumulatedQuantity += trade.Quantity;
            }
            else
            {
                AccumulatedQuantity -= trade.Quantity;

                //// TO DO: Dirty fix, potentially inaccurate with deposits and exchanges for another quote: i.e. buy with USDT, sell for BUSD -> not accounted for
                if (AccumulatedQuantity < 0)
                    AccumulatedQuantity = 0;

                if (AccumulatedQuantity == 0)
                    PositionPrice = 0;
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

        public decimal GetCurrentPositionValue(decimal currentPrice)
        {
            return AccumulatedQuantity * currentPrice;
        }
    }
}

