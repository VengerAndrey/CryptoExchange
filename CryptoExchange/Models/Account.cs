using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoExchange.Models
{
    public class Account
    {
        public int UserId { get; set; }
        public int CoinId { get; set; }
        public double Amount { get; set; }
    }
}
