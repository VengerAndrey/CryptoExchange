using System;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoExchange.Services
{
    public class SettingService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly List<Setting> _defaultSettings;

        public SettingService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;

            _defaultSettings = configuration
                .GetSection("Settings")
                .GetChildren()
                .Select(x => new Setting {Key = x.Key, Value = x.Value})
                .ToList();

            Reset();
        }

        public async void Reset()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Settings.RemoveRange(context.Settings);
            await context.Settings.AddRangeAsync(_defaultSettings);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public async void Set(string key, object value)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var setting = context.Settings.FirstOrDefault(x => x.Key == key);

            if (setting != null)
            {
                setting.Value = value.ToString();

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public string Get(string key)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var setting = context.Settings.FirstOrDefault(x => x.Key == key);

            return setting?.Value;
        }

        public int GetInt(string key)
        {
            var value = Get(key);

            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception e)
            {
                return default(int);
            }
        }

        public double GetDouble(string key)
        {
            var value = Get(key);

            try
            {
                return Convert.ToDouble(value);
            }
            catch (Exception e)
            {
                return default(double);
            }
        }
    }
}
