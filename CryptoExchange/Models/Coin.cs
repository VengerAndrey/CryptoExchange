using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoExchange.Models
{
    public class Coin
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double SellRate { get; set; }
        public double BuyRate { get; set; }
        public double Amount { get; set; }
    }
}
