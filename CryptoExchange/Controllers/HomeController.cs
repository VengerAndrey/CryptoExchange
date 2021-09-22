using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains("userId"))
            {
                var userId = HttpContext.Session.GetInt32("userId").Value;
                ViewData["userId"] = userId;
                var accounts = _context.Accounts.Include(x => x.Coin).Where(x => x.UserId == userId).ToList();
                var coins = _context.Coins.ToList();
                ViewData["accounts"] = accounts;
                ViewData["coins"] = coins;
            }

            return View();
        }
    }
}
