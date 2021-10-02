using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Models;

namespace CryptoExchange.Services
{
    public interface ICoinData
    {
        List<Coin> GetAll();
        List<Coin> GetAllAvailable();
    }
}
