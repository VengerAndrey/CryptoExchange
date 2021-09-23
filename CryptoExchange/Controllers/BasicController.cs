using CryptoExchange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CryptoExchange.Controllers
{
    public class BasicController : Controller
    {
        private readonly ILogger<BasicController> _logger;

        public BasicController(ILogger<BasicController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            ViewData["userId"] = userId;

            if (userId.HasValue)
            {
                return new RedirectResult("/Home");
            }

            return new RedirectResult("/Auth/SignIn");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
