using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Model
{
    public class TradeReportResult
    {
        public List<TradeReport> reports;

        public decimal totalBuys, totalSells;
        public decimal totalRealizedPNL;

        public TradeReportResult(List<TradeReport> reports)
        {
            this.reports = reports;
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
    }
}
