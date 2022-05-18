using Binance.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Model
{
    public class BinanceTradeWrapper
    {
        public readonly BinanceTrade trade;

        public BinanceTradeWrapper(BinanceTrade trade)
        {
            this.trade = trade;
        }

        public override bool Equals(object obj)
        {
            return obj is BinanceTradeWrapper other &&
                   trade.Id == other.trade.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(trade.Id);
        }
    }
}
