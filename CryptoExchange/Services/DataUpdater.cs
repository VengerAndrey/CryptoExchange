using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Data;
using Microsoft.EntityFrameworkCore;
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
            Init();

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
                    if (oldValue is null || oldValue.Amount == 0)
                    {
                        newCoin.Amount = Convert.ToInt32(_settingService.GetDouble("InitialPurchase") / newCoin.BuyRate);
                    }
                    else
                    {
                        newCoin.Amount = oldValue.Amount;
                    }
                    await context.Coins.AddAsync(newCoin, cancellationToken);
                }

                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }, cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(_settingService.GetInt("DataUpdateRate")));

            return Task.CompletedTask;
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
                coin.Amount = Convert.ToInt32(_settingService.GetDouble("InitialPurchase") / coin.BuyRate);
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
