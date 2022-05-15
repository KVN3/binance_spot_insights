using InsightsLibrary.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsightsLibrary.Model
{
    public class TradeReportResult
    {
        public List<TradeReport> reports;
        private readonly IBookService bookService;
        public decimal totalBuys, totalSells;
        public decimal totalRealizedPNL;
        public decimal totalFeesPaid;

        public TradeReportResult(List<TradeReport> reports, IBookService bookService)
        {
            this.reports = reports;
            this.bookService = bookService;
            this.SummarizeResults();
        }

        public void SummarizeResults()
        {
            foreach (var report in reports)
            {
                totalBuys += report.totalBuys;
                totalSells += report.totalSells;
                totalRealizedPNL += report.RealizedPNL;
            }
        }
        public async Task<decimal?> GetUnrealizedPNL()
        {
            if (reports.Count == 0)
                return 0;

            string symbol = reports[0].Symbol;

            var bookInformation = await bookService.GetSymbolInformation(symbol);
            var currentReport = reports[reports.Count - 1];

            return -1 * (currentReport.position.PositionValue - currentReport.position.GetCurrentPositionValue(bookInformation.BestBidPrice));
        }

        public async Task<decimal?> GetFeesPaid()
        {
            if (reports.Count == 0)
                return 0;

            foreach (var report in reports)
            {
                totalFeesPaid += report.totalFeeCost;
            }

            return totalFeesPaid;
        }
    }
}
