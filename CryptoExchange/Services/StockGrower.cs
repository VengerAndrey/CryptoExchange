using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoExchange.Services
{
    public class StockGrower : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        private readonly SettingService _settingService;

        public StockGrower(IServiceScopeFactory scopeFactory, SettingService settingService)
        {
            _scopeFactory = scopeFactory;
            _settingService = settingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(o => Grow(), cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(_settingService.GetInt("StockGrowRate")));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void Grow()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var coins = await context.Coins.ToListAsync();

            foreach (var coin in coins)
            {
                coin.Amount += _settingService.GetDouble("AdditionalPurchase") / coin.BuyRate;
                coin.Amount = Math.Round(coin.Amount, 2);
            }
            
            context.UpdateRange(coins);

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
