﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CryptoExchange.Common;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.EntityFrameworkCore;

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
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email &&
                                                                             x.Password == Hasher.Hash(user.Password));
            if (existingUser != null)
            {
                HttpContext.Session.SetInt32("userId", existingUser.Id);
                if (existingUser.IsAdmin)
                {
                    HttpContext.Session.SetString("isAdmin", "true");
                }
                return new RedirectResult("/");
            }

            return new RedirectResult("/Error");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (existingUser == null)
            {
                user.SignUpTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                user.Password = Hasher.Hash(user.Password);
                user.Balance += 100000;

                try
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

                    HttpContext.Session.SetInt32("userId", existingUser.Id);
                    if (existingUser.IsAdmin)
                    {
                        HttpContext.Session.SetString("isAdmin", "true");
                    }
                    return new RedirectResult("/");
                }
                catch (Exception e)
                {
                    return new RedirectResult("/Error");
                }
            }

            return new RedirectResult("/Error");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("isAdmin");
            return new RedirectResult("/");
        }
    }
}
