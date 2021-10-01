using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoExchange.Services
{
    public class DataUpdater : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICoinData _coinData;
        private Timer _timer;
        private readonly SettingService _settingService;

        public DataUpdater(IServiceScopeFactory scopeFactory, ICoinData coinData, SettingService settingService)
        {
            _scopeFactory = scopeFactory;
            _coinData = coinData;
            _settingService = settingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Update();

            _timer = new Timer(o => Update(), cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(_settingService.GetInt("DataUpdateRate")));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void Update()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var newCoins = await _coinData.GetAll();
            var coins = context.Coins.ToList();
            var toDelete = new List<Coin>();

            foreach (var coin in coins)
            {
                if (newCoins.FirstOrDefault(x => x.Id == coin.Id) is null)
                {
                    toDelete.Add(coin);
                }
            }
            context.Coins.RemoveRange(toDelete);

            foreach (var newCoin in newCoins)
            {
                var coin = coins.FirstOrDefault(x => !toDelete.Contains(x) && x.Id == newCoin.Id);

                if (coin is null)
                {
                    newCoin.Amount = Convert.ToInt32(_settingService.GetDouble("InitialPurchase") / newCoin.BuyRate);
                    await context.AddAsync(newCoin);
                }
                else
                {
                    coin.Rank = newCoin.Rank;
                    coin.BuyRate = newCoin.BuyRate;
                    coin.SellRate = newCoin.SellRate;
                    context.Coins.Update(coin);
                }
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
