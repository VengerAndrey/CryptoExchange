using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CryptoExchange.Data;
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
            ViewData["isAdmin"] = HttpContext.Session.GetString("isAdmin");
            if (HttpContext.Session.Keys.Contains("userId"))
            {
                var userId = HttpContext.Session.GetInt32("userId").Value;
                ViewData["userId"] = userId;
                var accounts = _context.Accounts.Include(x => x.Coin).Where(x => x.UserId == userId).ToList();
                var coins = _context.Coins.ToList();
                ViewData["accounts"] = accounts;
                ViewData["coins"] = coins;

                return View();
            }

            return new RedirectResult("/Auth/SignIn");
        }

        public IActionResult Refill()
        {
            if (HttpContext.Session.Keys.Contains("userId"))
            {
                return View();
            }

            return new RedirectResult("/Auth/SignIn");
        }

        [HttpPost]
        public IActionResult Refill([FromBody] double amount)
        {
            if (HttpContext.Session.Keys.Contains("userId"))
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("userId"));

                if (user is null)
                {
                    return Unauthorized();
                }

                user.Balance += amount;
                _context.Users.Update(user);

                try
                {
                    _context.SaveChanges();
                    return Ok($"Balance is successfully refilled for {amount}$.");
                }
                catch (Exception e)
                {
                    return BadRequest("Can't refill the balance.");
                }
            }

            return Unauthorized();
        }
    }
}
