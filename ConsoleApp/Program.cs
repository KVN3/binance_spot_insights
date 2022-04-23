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
            Console.WriteLine("Hello World!");
            
            var spotHistoryService = DIContainer.Instance.GetService<ISpotOrderHistoryService>();
            await spotHistoryService.GetRealizedPNL();
        }
    }
}
