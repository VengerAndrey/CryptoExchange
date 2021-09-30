using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoExchange.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public long SignUpTimestamp { get; set; }
        public double Balance { get; set; }
        public bool IsAdmin { get; set; }
    }
}
