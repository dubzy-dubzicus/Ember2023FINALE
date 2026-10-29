using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using Ember2023.Auth;
using LiteDB;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/clubs")]
    [Route("/api/clubs")]
    public class ClubsController : ControllerBase
    {
        private static readonly LiteDatabase ClubsDb =
            new(Path.Combine(Program.dataDir, "DBs", "Clubs.db"));

        [HttpGet("v1/mine")]
        public IActionResult GetMyClubs()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new
            {
                Results = new List<object>(),
                TotalResults = 0
            });
        }

        [HttpGet("v1/{clubId}")]
        public IActionResult GetClub(long clubId)
        {
            return Ok(new
            {
                clubId = clubId,
                name = "EMBER Club",
                description = "Welcome to EMBER",
                creatorAccountId = 1,
                isPublic = true,
                memberCount = 1
            });
        }

        [HttpGet("v1/{clubId}/members")]
        public IActionResult GetClubMembers(long clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new
            {
                Results = new List<object>(),
                TotalResults = 0
            });
        }

        [HttpGet("v1/{clubId}/bans")]
        public IActionResult GetClubBans(long clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new
            {
                Results = new List<object>(),
                TotalResults = 0
            });
        }

        [HttpPost("v1/{clubId}/ban")]
        public IActionResult BanFromClub(long clubId, [FromBody] ClubBanRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new { success = true });
        }

        [HttpDelete("v1/{clubId}/ban/{targetAccountId}")]
        public IActionResult UnbanFromClub(long clubId, long targetAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new { success = true });
        }

        [HttpGet("v1/{clubId}/isBanned")]
        public IActionResult IsBannedFromClub(long clubId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            return Ok(new { isBanned = false });
        }

        public class ClubBanRequest
        {
            public long AccountId { get; set; }
            public string? Reason { get; set; }
        }
    }
}
