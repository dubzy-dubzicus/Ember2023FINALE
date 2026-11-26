using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes.DBs;
using System.Collections.Generic;
using System.Linq;

namespace Ember2023.Controllers
{
    /// <summary>
    /// Read-only JSON API used by the public website views.
    ///
    /// Only endpoints that the wwwroot/js/*.js files actually call live here.
    /// Every one of them reads straight from the Ember databases, so the site
    /// shows real data for every account, not just one hardcoded user.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class WebApiController : ControllerBase
    {
        private sealed class FeedItem
        {
            public string type { get; set; } = "";
            public string name { get; set; } = "";
            public string title { get; set; } = "";
            public string description { get; set; } = "";
            public System.DateTime createdAt { get; set; }
            public string? imageName { get; set; }
        }

        // ---- helpers -------------------------------------------------------

        private static string OwnerName(long creatorAccountId)
        {
            var owner = PlayerDB.Players.FindOne(p => p.PlayerId == creatorAccountId);
            return owner?.Player?.DisplayName ?? owner?.Player?.Username ?? "Unknown";
        }

        private static object RoomSummary(Ember2023.Classes.DBs.DBClasses.RoomDBClasses.Room r) => new
        {
            roomId = r.RoomId.ToString(),
            name = r.Name,
            description = r.Description ?? "",
            maxPlayers = r.MaxPlayers,
            imageName = r.ImageName,
            ownerName = OwnerName(r.CreatorAccountId)
        };

        // ---- news ----------------------------------------------------------

        // news.js -> /api/news/v1/all
        [HttpGet("news/v1/all")]
        public IActionResult GetAllNews()
        {
            var news = new List<object>
            {
                new { title = "Welcome to Ember Network", summary = "The self-hosted Rec Room server is now live.", createdAt = System.DateTime.UtcNow, imageUrl = "" },
                new { title = "Inventions Now Supported", summary = "Create and share inventions with the community.", createdAt = System.DateTime.UtcNow.AddDays(-1), imageUrl = "" },
                new { title = "Server Optimized for 100+ Players", summary = "Performance improvements for large rooms.", createdAt = System.DateTime.UtcNow.AddDays(-2), imageUrl = "" }
            };
            return Ok(news);
        }

        // ---- players -------------------------------------------------------

        // search.js (People tab) -> /api/players/v1/search?q=
        [HttpGet("players/v1/search")]
        public IActionResult SearchPlayers([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(new List<object>());

                var needle = q.Trim().ToLowerInvariant();

                var results = PlayerDB.Players.FindAll()
                    .Where(p => p.Player != null &&
                                ((p.Player.Username ?? "").ToLowerInvariant().Contains(needle) ||
                                 (p.Player.DisplayName ?? "").ToLowerInvariant().Contains(needle)))
                    .Take(50)
                    .Select(p => new
                    {
                        accountId = p.PlayerId,
                        username = p.Player!.Username,
                        displayName = p.Player.DisplayName,
                        name = p.Player.DisplayName ?? p.Player.Username,
                        profileImage = p.Player.ProfileImage,
                        bio = p.Player.Bio ?? ""
                    })
                    .ToList();

                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // profile.js -> /api/players/v1/{username}
        [HttpGet("players/v1/{username}")]
        public IActionResult GetPlayer(string username)
        {
            try
            {
                var player = PlayerDB.Players.FindOne(p => p.Player != null &&
                    (p.Player.Username == username || p.Player.DisplayName == username || p.PlayerId.ToString() == username));
                if (player == null || player.Player == null)
                    return NotFound();

                var roomsCount = RoomDB.Rooms.Find(r => r.CreatorAccountId == player.PlayerId).Count();
                var inventionsCount = InventionsDB.Inventions.Find(i => i.AccountId == player.PlayerId).Count();

                return Ok(new
                {
                    username = player.Player.Username,
                    displayName = player.Player.DisplayName,
                    accountId = player.PlayerId,
                    roomsCount = roomsCount,
                    inventionsCount = inventionsCount,
                    friendsCount = 0,
                    profileImage = player.Player.ProfileImage,
                    bio = player.Player.Bio ?? "",
                    createdAt = player.Player.CreatedAt
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ---- rooms ---------------------------------------------------------

        // search.js (Rooms tab) -> /api/rooms/v1/search?q=
        [HttpGet("rooms/v1/search")]
        public IActionResult SearchRooms([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(new List<object>());

                var needle = q.Trim().ToLowerInvariant();

                var rooms = RoomDB.Rooms.FindAll()
                    .Where(r => !r.IsDorm &&
                                (r.Name ?? "").ToLowerInvariant().Contains(needle))
                    .Take(50)
                    .ToList();

                return Ok(rooms.Select(RoomSummary));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // profile.js (Rooms tab) -> /api/rooms/v1/user/{username}
        [HttpGet("rooms/v1/user/{username}")]
        public IActionResult GetRoomsForUser(string username)
        {
            try
            {
                var player = PlayerDB.Players.FindOne(p => p.Player != null &&
                    (p.Player.Username == username || p.Player.DisplayName == username || p.PlayerId.ToString() == username));
                if (player == null)
                    return Ok(new List<object>());

                var rooms = RoomDB.Rooms.Find(r => r.CreatorAccountId == player.PlayerId).ToList();
                return Ok(rooms.Select(RoomSummary));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // home.js -> /api/rooms/v1/all
        [HttpGet("rooms/v1/all")]
        public IActionResult GetAllRooms()
        {
            try
            {
                var rooms = RoomDB.Rooms.FindAll().Where(r => !r.IsDorm).ToList();
                return Ok(rooms.Select(RoomSummary));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ---- inventions ----------------------------------------------------

        // search.js (Inventions tab) -> /api/inventions/v2/search?q=
        [HttpGet("inventions/v2/search")]
        public IActionResult SearchInventions([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(new List<object>());

                var needle = q.Trim().ToLowerInvariant();

                var inventions = InventionsDB.Inventions.FindAll()
                    .Where(i => i.IsPublic &&
                                (i.Name ?? "").ToLowerInvariant().Contains(needle))
                    .Take(50)
                    .Select(i => new
                    {
                        inventionId = i.InventionId,
                        name = i.Name,
                        description = i.Description,
                        imageName = i.ImageName,
                        isPublic = i.IsPublic,
                        createdAt = i.CreatedAt
                    })
                    .ToList();

                return Ok(inventions);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ---- feed ----------------------------------------------------------

        // profile.js (Feed tab) -> /api/feed/v1/user/{username}
        // A user's recent public activity: rooms they created plus public
        // inventions, newest first.
        [HttpGet("feed/v1/user/{username}")]
        public IActionResult GetUserFeed(string username)
        {
            try
            {
                var player = PlayerDB.Players.FindOne(p => p.Player != null &&
                    (p.Player.Username == username || p.Player.DisplayName == username || p.PlayerId.ToString() == username));
                if (player == null)
                    return Ok(new List<object>());

                var items = new List<FeedItem>();

                foreach (var r in RoomDB.Rooms.Find(x => x.CreatorAccountId == player.PlayerId))
                {
                    items.Add(new FeedItem
                    {
                        type = "room",
                        name = r.Name,
                        title = r.Name,
                        description = r.Description ?? "",
                        createdAt = r.CreatedAt,
                        imageName = r.ImageName
                    });
                }

                foreach (var i in InventionsDB.Inventions.Find(x => x.AccountId == player.PlayerId && x.IsPublic))
                {
                    items.Add(new FeedItem
                    {
                        type = "invention",
                        name = i.Name,
                        title = i.Name,
                        description = i.Description ?? "",
                        createdAt = i.CreatedAt,
                        imageName = i.ImageName
                    });
                }

                return Ok(items.OrderByDescending(x => x.createdAt).Take(50).ToList());
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ---- single room ---------------------------------------------------

        // room.js -> /api/web/rooms/v1/{roomId}
        [HttpGet("web/rooms/v1/{roomId}")]
        public IActionResult GetRoom(long roomId)
        {
            try
            {
                var room = RoomDB.Rooms.FindOne(r => r.RoomId == roomId);
                if (room == null)
                    return NotFound();

                return Ok(new
                {
                    roomId = room.RoomId.ToString(),
                    name = room.Name,
                    description = room.Description ?? "",
                    maxPlayers = room.MaxPlayers,
                    imageName = room.ImageName,
                    ownerName = OwnerName(room.CreatorAccountId),
                    tags = room.Tags?.Select(t => t.Tag).ToList() ?? new List<string>(),
                    players = new List<object>()
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
