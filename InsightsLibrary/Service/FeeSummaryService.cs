using InsightsLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsightsLibrary.Service
{
    public class FeeSummaryService
    {
        private readonly TradeReport tradeReport;
        private readonly IBookService bookService;
        private Dictionary<string, decimal> priceMap = new Dictionary<string, decimal>();

        private decimal usdtValue;

        public FeeSummaryService(TradeReport tradeReport, IBookService bookService)
        {
            this.tradeReport = tradeReport;
            this.bookService = bookService;
        }

        public async Task<decimal> SummarizeFeeCost()
        {
            foreach (KeyValuePair<string, decimal> entry in this.tradeReport.fees)
            {
                usdtValue += await GetBestBidPrice(entry.Key) * entry.Value;
            }

            return usdtValue;
        }
        
        private async Task<decimal> GetBestBidPrice(string symbol)
        {
            if (!priceMap.ContainsKey(symbol))
            {
                var bookInformation = await bookService.GetSymbolInformation(symbol + "USDT");
                priceMap[symbol] = bookInformation.BestBidPrice;
            }

            return priceMap[symbol];
        }
    }
}
