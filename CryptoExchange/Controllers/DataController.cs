using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Data;
using CryptoExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("coins")]
        [Route("coins-buy")]
        public async Task<IActionResult> GetStockCoins()
        {
            var coins = await _context.Coins.OrderBy(x => x.Rank).ToListAsync();

            return new JsonResult(coins);
        }

        [HttpGet]
        [Route("coins-sell")]
        public async Task<IActionResult> GetPersonalCoins()
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var accounts = await _context.Accounts.Include(x => x.Coin)
                .Where(x => x.UserId == userId).ToListAsync();

            foreach (var account in accounts)
            {
                account.Coin.Amount = account.Amount;
            }

            var coins = accounts.Select(x => x.Coin).OrderBy(x => x.Rank).ToList();

            return new JsonResult(coins);
        }

        [HttpGet]
        [Route("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.Value);

            if (user == null)
            {
                return Unauthorized();
            }

            return new JsonResult(user.Balance);
        }

        [HttpGet]
        [Route("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var accounts = await _context.Accounts.Include(x => x.Coin).Where(x => x.UserId == userId).ToListAsync();

            return new JsonResult(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> ProceedTransaction([FromBody]Transaction transaction)
        {
            if (transaction.Amount == 0)
            {
                return BadRequest("Empty transaction.");
            }

            var userId = HttpContext.Session.GetInt32("userId");

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var coin = await _context.Coins.FirstOrDefaultAsync(x => x.Id == transaction.CoinId);
            var account =
                await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.CoinId == coin.Id);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.Value);

            if (coin == null)
            {
                return NotFound();
            }

            if (user == null)
            {
                return Unauthorized();
            }

            if (account == null)
            {
                account = new Account
                {
                    UserId = user.Id,
                    CoinId = coin.Id,
                    Amount = 0
                };

                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();

                account = await _context.Accounts.FirstOrDefaultAsync(x =>
                    x.UserId == user.Id && x.CoinId == coin.Id);
            }

            coin.Amount -= transaction.Amount;
            account.Amount += transaction.Amount;

            user.Balance -= Math.Round(transaction.Amount * (transaction.Amount > 0 ? coin.BuyRate : coin.SellRate), 2);

            if (coin.Amount < 0)
            {
                return BadRequest("The requested amount is too large.");
            }

            if (user.Balance < 0)
            {
                return BadRequest("You don't have enough balance to proceed with transaction.");
            }

            if (account.Amount < 0)
            {
                return BadRequest("You don't have enough coins.");
            }

            try
            {
                _context.Coins.Update(coin);
                if (account.Amount > 0)
                {
                    _context.Accounts.Update(account);
                }
                else
                {
                    _context.Accounts.Remove(account);
                }
                _context.Users.Update(user);

                transaction.UserId = user.Id;
                transaction.Rate = (transaction.Amount > 0) ? coin.BuyRate : coin.SellRate;
                transaction.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                await _context.Transactions.AddAsync(transaction);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return Problem("Something went wrong.");
            }
        }
    }
}
