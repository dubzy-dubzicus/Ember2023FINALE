using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/economy")]
    public class EconomyController : ControllerBase
    {
        private static readonly LiteDatabase EconomyDb =
            new(Path.Combine(Program.dataDir, "DBs", "Economy.db"));
        private static readonly ILiteCollection<TokenBalanceEntry> TokenBalances =
            EconomyDb.GetCollection<TokenBalanceEntry>("TokenBalances");
        private static readonly ILiteCollection<TokenTransactionEntry> TokenTransactions =
            EconomyDb.GetCollection<TokenTransactionEntry>("TokenTransactions");

        static EconomyController()
        {
            TokenBalances.EnsureIndex(t => t.AccountId);
            TokenTransactions.EnsureIndex(t => t.AccountId);
            TokenTransactions.EnsureIndex(t => t.CreatedAt);
        }

        public class TokenBalanceEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public int Tokens { get; set; } = 1000; // starting balance
            public int Tickets { get; set; } = 0;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }

        public class TokenTransactionEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public int Amount { get; set; }
            public string Type { get; set; } = ""; // purchase, earn, refund, daily_reward
            public string Description { get; set; } = "";
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        [HttpGet("v1/balance")]
        public IActionResult GetBalance()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var balance = TokenBalances.FindOne(t => t.AccountId == (long)id);
            if (balance == null)
            {
                balance = new TokenBalanceEntry { AccountId = (long)id, Tokens = 1000, Tickets = 0 };
                TokenBalances.Insert(balance);
            }

            return Ok(new
            {
                accountId = balance.AccountId,
                tokens = balance.Tokens,
                tickets = balance.Tickets,
                updatedAt = balance.UpdatedAt.ToString("O")
            });
        }

        [HttpGet("v1/transactions")]
        public IActionResult GetTransactions([FromQuery] int limit = 50)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var txs = TokenTransactions.Find(t => t.AccountId == (long)id)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToList();

            return Ok(new
            {
                Results = txs.Select(t => new
                {
                    id = t.Id,
                    amount = t.Amount,
                    type = t.Type,
                    description = t.Description,
                    createdAt = t.CreatedAt.ToString("O")
                }).ToList(),
                TotalResults = txs.Count
            });
        }

        [HttpPost("v1/add")]
        public IActionResult AddTokens([FromBody] AddTokensRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var balance = TokenBalances.FindOne(t => t.AccountId == (long)id);
            if (balance == null)
            {
                balance = new TokenBalanceEntry { AccountId = (long)id, Tokens = 1000, Tickets = 0 };
                TokenBalances.Insert(balance);
            }

            balance.Tokens += request.Amount;
            balance.UpdatedAt = DateTime.UtcNow;
            TokenBalances.Update(balance);

            TokenTransactions.Insert(new TokenTransactionEntry
            {
                AccountId = (long)id,
                Amount = request.Amount,
                Type = request.Type ?? "admin_add",
                Description = request.Description ?? "Tokens added",
                CreatedAt = DateTime.UtcNow
            });

            return Ok(new
            {
                accountId = balance.AccountId,
                tokens = balance.Tokens,
                added = request.Amount
            });
        }

        [HttpPost("v1/spend")]
        public IActionResult SpendTokens([FromBody] SpendTokensRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var balance = TokenBalances.FindOne(t => t.AccountId == (long)id);
            if (balance == null || balance.Tokens < request.Amount)
                return BadRequest(new { error = "Insufficient tokens" });

            balance.Tokens -= request.Amount;
            balance.UpdatedAt = DateTime.UtcNow;
            TokenBalances.Update(balance);

            TokenTransactions.Insert(new TokenTransactionEntry
            {
                AccountId = (long)id,
                Amount = -request.Amount,
                Type = request.Type ?? "purchase",
                Description = request.Description ?? "Item purchase",
                CreatedAt = DateTime.UtcNow
            });

            return Ok(new
            {
                accountId = balance.AccountId,
                tokens = balance.Tokens,
                spent = request.Amount,
                success = true
            });
        }

        public class AddTokensRequest
        {
            public int Amount { get; set; }
            public string? Type { get; set; }
            public string? Description { get; set; }
        }

        public class SpendTokensRequest
        {
            public int Amount { get; set; }
            public string? Type { get; set; }
            public string? Description { get; set; }
        }
    }
}
