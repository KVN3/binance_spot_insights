using InsightsLibrary;
using InsightsLibrary.Service;
using ConsoleApp.Infrastructure;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string symbol = "GMTUSDT";
            var spotHistoryService = DIContainer.Instance.GetService<ISpotOrderHistoryService>();
            var result = await spotHistoryService.GetTradeReports(symbol);

            Console.WriteLine("");

            foreach (var report in result.reports)
            {
                string message = $"{report.symbol} - {report.Key.ToString("dd-MM-yyyy")} | Realized PNL: {report.realizedPNL.ToString("0.##"),10} USDt | " +
                    $"Buys: {report.totalBuys.ToString("0.##"),10} | Sells: {report.totalSells.ToString("0.##"),10} | " +
                    $"BuyQty: {report.totalBuyQuantity.ToString("0.##"),10} | SellQty: {report.totalSellQuantity.ToString("0.##"),10} ";

                if (report.position.IsOpen())
                {
                    message += $"| Position size of {report.position.AccumulatedQuantity.ToString("0.##"),5} " +
                        $"at price of {report.position.PositionPrice.ToString("0.##")} USDt";
                }
                else
                {
                    message += "| No position left";
                }

                Console.WriteLine(message);
            }

            Console.WriteLine();
            Console.WriteLine("Summary: ");
            Console.WriteLine($"{symbol}              | Realized PNL: {result.totalRealizedPNL.ToString("0.##"),10} USDt | " +
                $"Buys: {result.totalBuys.ToString("0.##"),10} | Sells: {result.totalSells.ToString("0.##"),10}");

            Console.ReadKey();
        }
    }
}
