using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/friends")]
    public class FriendsController : ControllerBase
    {
        private static readonly LiteDatabase FriendsDb =
            new(Path.Combine(Program.dataDir, "DBs", "Friends.db"));
        private static readonly ILiteCollection<FriendEntry> Friends =
            FriendsDb.GetCollection<FriendEntry>("Friends");

        static FriendsController()
        {
            Friends.EnsureIndex(f => f.AccountId);
            Friends.EnsureIndex(f => f.FriendAccountId);
        }

        public class FriendEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public long FriendAccountId { get; set; }
            public string Status { get; set; } = "friends";
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        [HttpGet("v1/list")]
        public IActionResult GetFriends()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long callerId = id.Value;
            var friendships = Friends.Find(f => f.AccountId == callerId).ToList();
            var results = new List<object>();

            foreach (var f in friendships)
            {
                var friend = PlayerDB.Players.FindById(f.FriendAccountId);
                results.Add(new
                {
                    friendAccountId = f.FriendAccountId,
                    username = friend?.Player?.Username ?? "Unknown",
                    displayName = friend?.Player?.DisplayName ?? "Unknown",
                    status = f.Status,
                    createdAt = f.CreatedAt.ToString("O")
                });
            }

            return Ok(new { Results = results, TotalResults = results.Count });
        }

        [HttpPost("v1/add/{friendAccountId}")]
        public IActionResult AddFriend(long friendAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long callerId = id.Value;

            if (callerId == friendAccountId)
                return BadRequest(new { error = "Cannot add yourself" });

            var existing = Friends.FindOne(f => f.AccountId == callerId && f.FriendAccountId == friendAccountId);
            if (existing != null)
                return Ok(new { success = true, alreadyFriends = true });

            Friends.Insert(new FriendEntry { AccountId = callerId, FriendAccountId = friendAccountId });
            return Ok(new { success = true });
        }

        [HttpDelete("v1/remove/{friendAccountId}")]
        public IActionResult RemoveFriend(long friendAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long callerId = id.Value;
            Friends.DeleteMany(f => f.AccountId == callerId && f.FriendAccountId == friendAccountId);
            return Ok(new { success = true });
        }

        [HttpGet("v1/search/{username}")]
        public IActionResult SearchUsers(string username)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long callerId = id.Value;
            var players = PlayerDB.Players.FindAll()
                .Where(p => p.Player != null &&
                           p.Player.Username != null &&
                           p.Player.Username.Contains(username, StringComparison.OrdinalIgnoreCase) &&
                           p.PlayerId != callerId)
                .Take(20)
                .ToList();

            return Ok(new
            {
                Results = players.Select(p => new
                {
                    accountId = p.PlayerId,
                    username = p.Player?.Username ?? "",
                    displayName = p.Player?.DisplayName ?? ""
                }).ToList(),
                TotalResults = players.Count
            });
        }
    }
}
