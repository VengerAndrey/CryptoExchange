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

            return View();
        }

        [HttpGet]
        public IActionResult RateMargin()
        {
            return Ok(_settingService.GetDouble("RateMargin"));
        }

        [HttpPost]
        public IActionResult RateMargin([FromBody] double rateMargin)
        {
            _settingService.Set("RateMargin", rateMargin);

            return Ok();
        }

        [HttpGet]
        public IActionResult RandomMargin()
        {
            return Ok(_settingService.GetDouble("RandomMargin"));
        }

        [HttpPost]
        public IActionResult RandomMargin([FromBody] double randomMargin)
        {
            _settingService.Set("RandomMargin", randomMargin);

            return Ok();
        }

        [HttpGet]
        public IActionResult InitialPurchase()
        {
            return Ok(_settingService.GetDouble("InitialPurchase"));
        }

        [HttpPost]
        public IActionResult InitialPurchase([FromBody] double initialPurchase)
        {
            _settingService.Set("InitialPurchase", initialPurchase);

            return Ok();
        }

        [HttpGet]
        public IActionResult AdditionalPurchase()
        {
            return Ok(_settingService.GetDouble("AdditionalPurchase"));
        }

        [HttpPost]
        public IActionResult AdditionalPurchase([FromBody] double additionalPurchase)
        {
            _settingService.Set("AdditionalPurchase", additionalPurchase);

            return Ok();
        }

        [HttpGet]
        public IActionResult InitialGrant()
        {
            return Ok(_settingService.GetDouble("InitialGrant"));
        }

        [HttpPost]
        public IActionResult InitialGrant([FromBody] double initialGrant)
        {
            _settingService.Set("InitialGrant", initialGrant);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Coins()
        {
            var coins = await _coinData.GetAll();

            return Json(coins);
        }

        [HttpGet]
        public async Task<IActionResult> AvailableCoins()
        {
            var coins = await _coinData.GetAllAvailable();

            return Json(coins);
        }

        [HttpPost]
        public IActionResult AddCoin([FromBody] string id)
        {
            var added = _exchangeCoinService.AddExchangeCoin(id);

            if (added)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete]
        public IActionResult DeleteCoin([FromBody] string id)
        {
            var deleted = _exchangeCoinService.DeleteExchangeCoin(id);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
