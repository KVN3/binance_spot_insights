using InsightsLibrary;
using InsightsLibrary.Service;
using ConsoleApp.Infrastructure;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using DataRepository;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            new ConnectivityTest().TestConnectivity();

            Console.ReadKey();
        }

        private static async Task WriteAllReports()
        {
            List<string> symbols = new List<string>()
            {
                "GMTUSDT",
                "ALGOUSDT",
                "APEUSDT",
                "COMPUSDT"
            };

            foreach (string symbol in symbols)
            {
                await WriteConsoleReport(symbol);
            }
        }

        private static async Task WriteConsoleReport(string symbol)
        {
            var spotHistoryService = DIContainer.Instance.GetService<ISpotOrderHistoryService>();
            var result = await spotHistoryService.GetTradeReports(symbol);

            Console.WriteLine("");

            foreach (var report in result.reports)
            {
                string message = $"{report.Symbol} - {report.Key.ToString("dd-MM-yyyy")} | Realized PNL: {report.RealizedPNL.ToString("0.##"),10} USDt | " +
                    $"Buys: {report.totalBuys.ToString("0.##"),10} | Sells: {report.totalSells.ToString("0.##"),10} | " +
                    $"BuyQty: {report.totalBuyQuantity.ToString("0.##"),10} | SellQty: {report.totalSellQuantity.ToString("0.##"),10} ";

                if (report.position.IsOpen())
                {
                    message += $"| Position size of {report.position.AccumulatedQuantity.ToString("0.##"),5} " +
                        $"at price of {report.position.PositionPrice.ToString("0.####")} USDt";
                }
                else
                {
                    message += "| No position left";
                }

                Console.WriteLine(message);
            }

            decimal? unrealizedPNL = await result.GetUnrealizedPNL();
            string unrealizedPNLMessage = "-";
            if (unrealizedPNL != null)
            {
                decimal uPNL = unrealizedPNL ?? 0;
                unrealizedPNLMessage = uPNL.ToString("0.##");
            }

            Console.WriteLine();
            Console.WriteLine("Summary: ");
            Console.WriteLine($"{symbol}              | Realized PNL: {result.totalRealizedPNL.ToString("0.##"),10} USDt | " +
                $"Unrealized PNL: {unrealizedPNLMessage,10} USDt | " +
                $"Buys: {result.totalBuys.ToString("0.##"),10} | Sells: {result.totalSells.ToString("0.##"),10}");
        }
    }
}
