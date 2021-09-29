using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoExchange.Services
{
    public class DataUpdater : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICoinData _coinData;
        private Timer _timer;
        private readonly double _initialPurchase;
        private readonly int _dataUpdateRate;

        public DataUpdater(IServiceScopeFactory scopeFactory, ICoinData coinData, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _coinData = coinData;
            _initialPurchase = configuration.GetValue<double>("InitialPurchase");
            _dataUpdateRate = configuration.GetValue<int>("DataUpdateRate");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Init();

            await Task.Delay(1000, cancellationToken);

            _timer = new Timer(async o =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var newCoins = await _coinData.GetAll();
                var coins = context.Coins.ToList();
                var values = context.Coins.Select(x => new {Id = x.Id, Amount = x.Amount});

                context.Coins.RemoveRange(coins);
                foreach (var newCoin in newCoins)
                {
                    var oldValue = await values.FirstOrDefaultAsync(x => x.Id == newCoin.Id, cancellationToken);
                    if (oldValue is null)
                    {
                        newCoin.Amount = Convert.ToInt32(_initialPurchase / newCoin.BuyRate);
                    }
                    else
                    {
                        newCoin.Amount = oldValue.Amount;
                    }
                    await context.Coins.AddAsync(newCoin, cancellationToken);
                }

                /*foreach (var coin in coins)
                {
                    var newCoin = newCoins.FirstOrDefault(x => x.Id == coin.Id);

                    if (newCoin is null)
                    {
                        context.Coins.Remove(coin);
                    }
                    else
                    {
                        coin.SellRate = newCoin.SellRate;
                        coin.BuyRate = newCoin.BuyRate;
                        coin.Rank = newCoin.Rank;
                        coin.Name = newCoin.Name;
                    }
                }

                foreach (var newCoin in newCoins)
                {
                    var coin = coins.FirstOrDefault(x => x.Id == newCoin.Id);

                    if (coin is null)
                    {
                        newCoin.Amount = Convert.ToInt32(_initialPurchase / newCoin.BuyRate);
                        await context.Coins.AddAsync(newCoin, cancellationToken);
                    }
                }

                context.Coins.UpdateRange(coins);*/

                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }, cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(_dataUpdateRate));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void Init()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Coins.Any())
            {
                context.Coins.RemoveRange(context.Coins);
                await context.SaveChangesAsync();
            }

            var coins = await _coinData.GetAll();

            foreach (var coin in coins)
            {
                coin.Amount = Convert.ToInt32(_initialPurchase / coin.BuyRate);
                await context.Coins.AddAsync(coin);
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
