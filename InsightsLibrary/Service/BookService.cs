using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using InsightsLibrary.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightsLibrary.Service
{
    public interface IBookService
    {
        Task<BinanceBookPrice> GetSymbolInformation(string symbol);
    }

    public class BookService : BinanceService, IBookService
    {
        public BookService(IBinanceClient binanceClient, ILogger<BookService> logger) : base(binanceClient, logger)
        {

        }

        public async Task<BinanceBookPrice> GetSymbolInformation(string symbol)
        {
            var response = await binanceClient.SpotApi.ExchangeData.GetBookPriceAsync(symbol);
            response.GetResultOrError(out BinanceBookPrice result, out Error error);

            return result;
        }
    }
}