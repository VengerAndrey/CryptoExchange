using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoExchange.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CoinId { get; set; }
        // if + => user bought, if - => user sold
        public double Amount { get; set; }
        public double Rate { get; set; }
        public long Timestamp { get; set; }
    }
}
