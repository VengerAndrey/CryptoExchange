using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Common;
using CryptoExchange.Models;
using CryptoExchange.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace CryptoExchange.Services
{
    public class NomicsData : ICoinData
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly int _apiDelay;
        private readonly double _rateDelta;
        private readonly double _randomDelta;
        private readonly Random _random;

        public NomicsData(IConfiguration configuration)
        {
            _apiKey = configuration["ApiKey"];
            _apiDelay = configuration.GetValue<int>("ApiDelay");
            _rateDelta = configuration.GetValue<double>("RateDelta");
            _randomDelta = configuration.GetValue<double>("RandomDelta");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(@"https://api.nomics.com/v1/")
            };
            _random = new Random();
        }

        public async Task<List<Coin>> GetAll()
        {
            await Task.Delay(_apiDelay);

            var uri = new Uri(_httpClient.BaseAddress + "currencies/ticker")
                .AddParameter("key", _apiKey)
                .AddParameter("ids", "BTC,ETH,HEX,ADA,USDT,BNB,XRP,SOL,DOT,USDC");
            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var coinTickers = await response.Content.ReadAsAsync<List<CoinTicker>>();

                var coins = coinTickers.Select(x => new Coin
                {
                    Id = x.Id,
                    Name = x.Name,
                    BuyRate = Math.Round(x.Price * (1 + _rateDelta + (_random.Next() % 2 == 0 ? _randomDelta : -_randomDelta)), 2) ,
                    SellRate = Math.Round(x.Price * (1 - _rateDelta + (_random.Next() % 2 == 0 ? _randomDelta : -_randomDelta)), 2),
                    Amount = 0,
                    Rank = x.Rank
                }).ToList();

                return coins;
            }

            return new List<Coin>();
        }
    }
}
