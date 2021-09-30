using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoExchange.Common;
using CryptoExchange.Models;
using CryptoExchange.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace CryptoExchange.Services
{
    public class NomicsData : ICoinData
    {
        private readonly ExchangeCoinService _exchangeCoinService;
        private readonly SettingService _settingService;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly Random _random;

        public NomicsData(ExchangeCoinService exchangeCoinService, IConfiguration configuration, SettingService settingService)
        {
            _exchangeCoinService = exchangeCoinService;
            _settingService = settingService;
            _apiKey = configuration["ApiKey"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(@"https://api.nomics.com/v1/")
            };
            _random = new Random();
        }

        public async Task<List<Coin>> GetAll()
        {
            await Task.Delay(_settingService.GetInt("ApiDelay"));
            var uri = new Uri(_httpClient.BaseAddress + "currencies/ticker")
                .AddParameter("key", _apiKey)
                .AddParameter("ids", _exchangeCoinService.GetExchangeCoinsString());
            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var coinTickers = await response.Content.ReadAsAsync<List<CoinTicker>>();
                var rateMargin = _settingService.GetDouble("RateMargin");
                var randomMargin = _settingService.GetDouble("RandomMargin");

                var coins = coinTickers.Select(x => new Coin
                {
                    Id = x.Id,
                    Name = x.Name,
                    BuyRate = Math.Round(x.Price * (1 + rateMargin + (_random.Next() % 2 == 0 ? randomMargin : -randomMargin)), 2) ,
                    SellRate = Math.Round(x.Price * (1 - rateMargin + (_random.Next() % 2 == 0 ? randomMargin : -randomMargin)), 2),
                    Amount = 0,
                    Rank = x.Rank
                }).ToList();

                return coins;
            }

            return new List<Coin>();
        }
    }
}
