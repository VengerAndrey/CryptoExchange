using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CryptoExchange.Services;
using Microsoft.AspNetCore.Http;

namespace CryptoExchange.Controllers
{
    public class AdminController : Controller
    {
        private readonly SettingService _settingService;
        private readonly ExchangeCoinService _exchangeCoinService;
        private readonly ICoinData _coinData;

        public AdminController(SettingService settingService, ExchangeCoinService exchangeCoinService, ICoinData coinData)
        {
            _settingService = settingService;
            _exchangeCoinService = exchangeCoinService;
            _coinData = coinData;
        }

        public IActionResult Index()
        {
            ViewData["userId"] = HttpContext.Session.GetInt32("userId");
            ViewData["isAdmin"] = HttpContext.Session.GetString("isAdmin");

            if (ViewData["userId"] != null && ViewData["isAdmin"] != null)
            {
                return View();
            }

            return new RedirectResult("/Home");
        }

        [HttpGet]
        public IActionResult RateMargin()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                return Ok(_settingService.GetDouble("RateMargin"));
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult RateMargin([FromBody] double rateMargin)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                _settingService.Set("RateMargin", rateMargin);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult RandomMargin()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                return Ok(_settingService.GetDouble("RandomMargin"));
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult RandomMargin([FromBody] double randomMargin)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                _settingService.Set("RandomMargin", randomMargin);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult InitialPurchase()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                return Ok(_settingService.GetDouble("InitialPurchase"));
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult InitialPurchase([FromBody] double initialPurchase)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                _settingService.Set("InitialPurchase", initialPurchase);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult AdditionalPurchase()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                return Ok(_settingService.GetDouble("AdditionalPurchase"));
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult AdditionalPurchase([FromBody] double additionalPurchase)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                _settingService.Set("AdditionalPurchase", additionalPurchase);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult InitialGrant()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                return Ok(_settingService.GetDouble("InitialGrant"));
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult InitialGrant([FromBody] double initialGrant)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                _settingService.Set("InitialGrant", initialGrant);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> Coins()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                var coins = _coinData.GetAll();

                return Json(coins);
            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> AvailableCoins()
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                var coins = _coinData.GetAllAvailable();

                return Json(coins);
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult AddCoin([FromBody] string id)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                var added = _exchangeCoinService.AddExchangeCoin(id);

                if (added)
                {
                    return Ok();
                }

                return NotFound();
            }

            return Unauthorized();
        }

        [HttpDelete]
        public IActionResult DeleteCoin([FromBody] string id)
        {
            if (HttpContext.Session.Keys.Contains("isAdmin") && HttpContext.Session.GetString("isAdmin") == "true")
            {
                var deleted = _exchangeCoinService.DeleteExchangeCoin(id);

                if (deleted)
                {
                    return NoContent();
                }

                return NotFound();
            }

            return Unauthorized();
        }
    }
}
