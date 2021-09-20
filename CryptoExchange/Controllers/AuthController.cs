using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CryptoExchange.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(User user)
        {
            // implement hashing
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email &&
                                                                             x.Password == user.Password);
            if (existingUser != null)
            {
                return new RedirectResult("/");
            }

            return new RedirectResult("/Error");
        }
    }
}
