using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoExchange.Services
{
    public class ExchangeCoinService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly List<ExchangeCoin> _allExchangeCoins;

        public ExchangeCoinService(IServiceScopeFactory scopeFactory,  IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            //_allExchangeCoins = configuration.GetValue<string[]>("CoinIds").Select(x => new ExchangeCoin {CoinId = x}).ToList();
            _allExchangeCoins = configuration
                .GetSection("CoinIds")
                .GetChildren()
                .Select(x => new ExchangeCoin {CoinId = x.Value})
                .ToList();
        }

        public List<ExchangeCoin> GetExchangeCoins()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return context.ExchangeCoins.ToList();
        }

        public List<ExchangeCoin> GetAvailableExchangeCoins()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return _allExchangeCoins.Except(context.ExchangeCoins).ToList();
        }

        public bool AddExchangeCoin(string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var exchangeCoin = _allExchangeCoins.FirstOrDefault(x => x.CoinId == id);

            if (exchangeCoin != null)
            {
                context.ExchangeCoins.Add(exchangeCoin);

                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public string GetExchangeCoinsString()
        {
            var coins = GetExchangeCoins();

            var result = "";

            foreach (var exchangeCoin in coins)
            {
                result += exchangeCoin.CoinId + ",";
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }
    }
}
