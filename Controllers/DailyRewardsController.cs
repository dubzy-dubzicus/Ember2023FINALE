using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using Ember2023.Controllers;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/daily")]
    public class DailyRewardsController : ControllerBase
    {
        private static readonly LiteDatabase DailyDb =
            new(Path.Combine(Program.dataDir, "DBs", "DailyRewards.db"));
        private static readonly ILiteCollection<DailyRewardEntry> DailyRewards =
            DailyDb.GetCollection<DailyRewardEntry>("DailyRewards");
        private static readonly object DailyLock = new();

        static DailyRewardsController()
        {
            DailyRewards.EnsureIndex(d => d.AccountId);
        }

        public class DailyRewardEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public int StreakCount { get; set; } = 0;
            public DateTime LastClaimedAt { get; set; } = DateTime.MinValue;
            public int TotalClaims { get; set; } = 0;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        // Daily reward tiers: day 1-7+
        private static readonly (int Day, int Tokens, int Tickets)[] RewardTiers = new[]
        {
            (1, 100, 0),
            (2, 150, 0),
            (3, 200, 1),
            (4, 250, 1),
            (5, 300, 2),
            (6, 400, 2),
            (7, 500, 5),
        };

        // Streak bonus multipliers: every 7-day streak milestone
        private static readonly (int StreakDays, double Multiplier)[] StreakBonuses = new[]
        {
            (7, 1.5),
            (14, 2.0),
            (21, 2.5),
            (28, 3.0),
            (35, 3.5),
            (42, 4.0),
            (49, 4.5),
            (50, 5.0),
        };

        [HttpGet("v1/status")]
        public IActionResult GetStatus()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var reward = DailyRewards.FindOne(d => d.AccountId == (long)id);
            if (reward == null)
            {
                reward = new DailyRewardEntry { AccountId = (long)id, StreakCount = 0, LastClaimedAt = DateTime.MinValue };
                DailyRewards.Insert(reward);
            }

            bool canClaim = CanClaim(reward.LastClaimedAt);
            int nextDay = Math.Min(reward.StreakCount + 1, RewardTiers.Length);
            double nextMultiplier = 1.0;
            foreach (var bonus in StreakBonuses.OrderByDescending(b => b.StreakDays))
            {
                if (nextDay >= bonus.StreakDays)
                {
                    nextMultiplier = bonus.Multiplier;
                    break;
                }
            }

            return Ok(new
            {
                accountId = reward.AccountId,
                streakCount = reward.StreakCount,
                totalClaims = reward.TotalClaims,
                lastClaimedAt = reward.LastClaimedAt == DateTime.MinValue ? null : reward.LastClaimedAt.ToString("O"),
                canClaim = canClaim,
                nextDayTokens = canClaim ? RewardTiers[Math.Min(nextDay - 1, RewardTiers.Length - 1)].Tokens : 0,
                nextDayTickets = canClaim ? RewardTiers[Math.Min(nextDay - 1, RewardTiers.Length - 1)].Tickets : 0,
                nextDayMultiplier = nextMultiplier,
            });
        }

        [HttpPost("v1/claim")]
        public IActionResult ClaimReward()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;
            var reward = DailyRewards.FindOne(d => d.AccountId == accountId);
            if (reward == null)
            {
                reward = new DailyRewardEntry { AccountId = accountId, StreakCount = 0, LastClaimedAt = DateTime.MinValue };
                DailyRewards.Insert(reward);
            }

            if (!CanClaim(reward.LastClaimedAt))
                return BadRequest(new { error = "Already claimed today" });

            // Calculate streak
            int newStreak = reward.LastClaimedAt == DateTime.MinValue ? 1 : reward.StreakCount + 1;
            int dayIndex = Math.Min(newStreak - 1, RewardTiers.Length - 1);
            int baseTokens = RewardTiers[dayIndex].Tokens;
            int baseTickets = RewardTiers[dayIndex].Tickets;

            // Calculate streak bonus multiplier
            double streakMultiplier = 1.0;
            foreach (var bonus in StreakBonuses.OrderByDescending(b => b.StreakDays))
            {
                if (newStreak >= bonus.StreakDays)
                {
                    streakMultiplier = bonus.Multiplier;
                    break;
                }
            }

            int tokens = (int)(baseTokens * streakMultiplier);
            int tickets = baseTickets;

            reward.StreakCount = newStreak;
            reward.LastClaimedAt = DateTime.UtcNow;
            reward.TotalClaims++;
            DailyRewards.Update(reward);

            // Add tokens to economy
            lock (DailyLock)
            {
                var economyDb = new LiteDatabase(Path.Combine(Program.dataDir, "DBs", "Economy.db"));
                var balances = economyDb.GetCollection<BsonDocument>("TokenBalances");
                var existing = balances.Find(Query.EQ("AccountId", accountId)).FirstOrDefault();
                int totalTokens = tokens;
                if (existing == null)
                {
                    var doc = new BsonDocument
                    {
                        { "AccountId", accountId },
                        { "Tokens", tokens },
                        { "Tickets", tickets },
                        { "UpdatedAt", DateTime.UtcNow }
                    };
                    balances.Insert(doc);
                }
                else
                {
                    existing["Tokens"] = existing["Tokens"].AsInt32 + tokens;
                    existing["Tickets"] = existing["Tickets"].AsInt32 + tickets;
                    existing["UpdatedAt"] = DateTime.UtcNow;
                    balances.Update(existing);
                    totalTokens = existing["Tokens"].AsInt32;
                }
                economyDb.Dispose();

                return Ok(new
                {
                    accountId = accountId,
                    streakCount = newStreak,
                    tokensEarned = tokens,
                    baseTokens = baseTokens,
                    streakMultiplier = streakMultiplier,
                    ticketsEarned = tickets,
                    totalTokens = totalTokens,
                    claimedAt = reward.LastClaimedAt.ToString("O")
                });
            }
        }

        private static bool CanClaim(DateTime lastClaimed)
        {
            if (lastClaimed == DateTime.MinValue)
                return true;

            var now = DateTime.UtcNow;
            var lastUtc = DateTime.SpecifyKind(lastClaimed, DateTimeKind.Utc);
            // Can claim once per UTC day
            return lastUtc.Date < now.Date;
        }
    }
}
