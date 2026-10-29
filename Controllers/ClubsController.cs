using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/clubs")]
    public class ClubsController : ControllerBase
    {
        private static readonly LiteDatabase ClubsDb =
            new(Path.Combine(Program.dataDir, "DBs", "EmberClubs.db"));
private static readonly ILiteCollection<ClubEntry> Clubs =
    ClubsDb.GetCollection<ClubEntry>("Clubs");
private static readonly ILiteCollection<ClubMemberEntry> ClubMembers =
    ClubsDb.GetCollection<ClubMemberEntry>("ClubMembers");
private static readonly ILiteCollection<ClubBanEntry> ClubBans =
    ClubsDb.GetCollection<ClubBanEntry>("ClubBans");

        static ClubsController()
        {
            Clubs.EnsureIndex(c => c.CreatorAccountId);
            ClubMembers.EnsureIndex(cm => cm.ClubId);
            ClubMembers.EnsureIndex(cm => cm.AccountId);
            ClubBans.EnsureIndex(cb => cb.ClubId);
            ClubBans.EnsureIndex(cb => cb.AccountId);
        }

        public class ClubEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string ClubId { get; set; } = Guid.NewGuid().ToString("N");
            public long CreatorAccountId { get; set; }
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public bool IsPublic { get; set; } = true;
            public int MemberCount { get; set; } = 1;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class ClubMemberEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string ClubId { get; set; } = "";
            public long AccountId { get; set; }
            public string Role { get; set; } = "member";
            public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        }

        public class ClubBanEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string ClubId { get; set; } = "";
            public long AccountId { get; set; }
            public string Reason { get; set; } = "";
            public DateTime BannedAt { get; set; } = DateTime.UtcNow;
        }

        public class CreateClubRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool IsPublic { get; set; } = true;
        }

        [HttpGet("v1/mine")]
        public IActionResult GetMyClubs()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var memberships = ClubMembers.Find(cm => cm.AccountId == (long)id).ToList();
            var results = new List<object>();

            foreach (var m in memberships)
            {
                var club = Clubs.FindOne(c => c.ClubId == m.ClubId);
                if (club != null)
                {
                    results.Add(new
                    {
                        clubId = club.ClubId,
                        name = club.Name,
                        description = club.Description,
                        isPublic = club.IsPublic,
                        memberCount = club.MemberCount,
                        role = m.Role,
                        createdAt = club.CreatedAt.ToString("O")
                    });
                }
            }

            return Ok(new { Results = results, TotalResults = results.Count });
        }

        [HttpPost("v1/create")]
        public IActionResult CreateClub([FromBody] CreateClubRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var club = new ClubEntry
            {
                CreatorAccountId = (long)id,
                Name = request.Name ?? "New Club",
                Description = request.Description ?? "",
                IsPublic = request.IsPublic,
                MemberCount = 1
            };
            Clubs.Insert(club);

            ClubMembers.Insert(new ClubMemberEntry
            {
                ClubId = club.ClubId,
                AccountId = (long)id,
                Role = "owner",
                JoinedAt = DateTime.UtcNow
            });

            return Ok(new
            {
                clubId = club.ClubId,
                name = club.Name,
                description = club.Description,
                isPublic = club.IsPublic,
                role = "owner"
            });
        }

        [HttpGet("v1/{clubId}")]
        public IActionResult GetClub(string clubId)
        {
            var club = Clubs.FindOne(c => c.ClubId == clubId);
            if (club == null)
                return NotFound();

            return Ok(new
            {
                clubId = club.ClubId,
                name = club.Name,
                description = club.Description,
                isPublic = club.IsPublic,
                memberCount = club.MemberCount,
                creatorAccountId = club.CreatorAccountId,
                createdAt = club.CreatedAt.ToString("O")
            });
        }

        [HttpGet("v1/{clubId}/isBanned")]
        public IActionResult IsBannedFromClub(string clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var ban = ClubBans.FindOne(cb => cb.ClubId == clubId && cb.AccountId == (long)id);
            return Ok(new { isBanned = ban != null });
        }

        [HttpGet("v1/{clubId}/members")]
        public IActionResult GetClubMembers(string clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var members = ClubMembers.Find(cm => cm.ClubId == clubId).ToList();
            var results = new List<object>();

            foreach (var m in members)
            {
                var player = PlayerDB.Players.FindById(m.AccountId);
                results.Add(new
                {
                    accountId = m.AccountId,
                    username = player?.Player?.Username ?? "Unknown",
                    role = m.Role,
                    joinedAt = m.JoinedAt.ToString("O")
                });
            }

            return Ok(new { Results = results, TotalResults = results.Count });
        }

        [HttpPost("v1/{clubId}/join")]
        public IActionResult JoinClub(string clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var existing = ClubMembers.FindOne(cm => cm.ClubId == clubId && cm.AccountId == (long)id);
            if (existing != null)
                return Ok(new { success = true, alreadyMember = true });

            ClubMembers.Insert(new ClubMemberEntry
            {
                ClubId = clubId,
                AccountId = (long)id,
                Role = "member",
                JoinedAt = DateTime.UtcNow
            });

            var club = Clubs.FindOne(c => c.ClubId == clubId);
            if (club != null)
            {
                club.MemberCount++;
                Clubs.Update(club);
            }

            return Ok(new { success = true });
        }

        [HttpPost("v1/{clubId}/leave")]
        public IActionResult LeaveClub(string clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            ClubMembers.DeleteMany(cm => cm.ClubId == clubId && cm.AccountId == (long)id);

            var club = Clubs.FindOne(c => c.ClubId == clubId);
            if (club != null && club.MemberCount > 0)
            {
                club.MemberCount--;
                Clubs.Update(club);
            }

            return Ok(new { success = true });
        }
    }
}
