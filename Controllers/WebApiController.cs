using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes.DBs;
using Ember2023.Classes.DBs.DBClasses;
using System.Collections.Generic;
using System.Linq;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("api")]
    public class WebApiController : ControllerBase
    {
        [HttpGet("rooms/v1/all")]
        public IActionResult GetAllRooms()
        {
            try
            {
                var rooms = RoomDB.Rooms.FindAll().ToList();
                return Ok(rooms.Select(r => new
                {
                    roomId = r.RoomId.ToString(),
                    name = r.Name,
                    description = r.Description ?? "",
                    maxPlayers = r.MaxPlayers,
                    imageName = r.ImageName
                }));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

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
                    ownerName = "Unknown",
                    tags = new List<string>()
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("rooms/v1/search")]
        public IActionResult SearchRooms([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                    return Ok(new List<object>());

                var rooms = RoomDB.Rooms.Find(r => r.Name.Contains(q)).ToList();
                return Ok(rooms.Select(r => new
                {
                    roomId = r.RoomId.ToString(),
                    name = r.Name,
                    description = r.Description ?? ""
                }));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("rooms/v1/user/{username}")]
        public IActionResult GetUserRooms(string username)
        {
            try
            {
                var player = PlayerDB.Players.FindOne(p => p.Player != null && (p.Player.Username == username || p.Player.DisplayName == username || p.PlayerId.ToString() == username));
                if (player == null || player.Player == null)
                    return Ok(new List<object>());

                var rooms = RoomDB.Rooms.Find(r => r.CreatorAccountId == player.PlayerId).ToList();
                return Ok(rooms.Select(r => new
                {
                    roomId = r.RoomId.ToString(),
                    name = r.Name,
                    description = r.Description ?? "",
                    maxPlayers = r.MaxPlayers,
                    imageName = r.ImageName
                }));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("news/v1/all")]
        public IActionResult GetAllNews()
        {
            var news = new List<object>
            {
                new { Title = "Welcome to Ember Network", Summary = "The self-hosted Rec Room server is now live.", CreatedAt = System.DateTime.UtcNow, ImageUrl = "" },
                new { Title = "Inventions Now Supported", Summary = "Create and share inventions with the community.", CreatedAt = System.DateTime.UtcNow.AddDays(-1), ImageUrl = "" },
                new { Title = "Server Optimized for 100+ Players", Summary = "Performance improvements for large rooms.", CreatedAt = System.DateTime.UtcNow.AddDays(-2), ImageUrl = "" }
            };
            return Ok(news);
        }

        [HttpGet("players/v1/search")]
        public IActionResult SearchPlayers([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                    return Ok(new List<object>());

                var players = PlayerDB.Players.FindAll().ToList();
                var results = players
                    .Where(p => p.Player != null && (
                        (p.Player.Username ?? "").Contains(q, System.StringComparison.OrdinalIgnoreCase) || 
                        (p.Player.DisplayName ?? "").Contains(q, System.StringComparison.OrdinalIgnoreCase) ||
                        p.PlayerId.ToString().Contains(q)))
                    .Select(p => new
                    {
                        username = p.Player.Username ?? "Unknown",
                        name = p.Player.DisplayName ?? p.Player.Username ?? "Unknown",
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

        [HttpGet("players/v1/{username}")]
        public IActionResult GetPlayer(string username)
        {
            try
            {
                var player = PlayerDB.Players.FindOne(p => p.Player != null && (p.Player.Username == username || p.Player.DisplayName == username || p.PlayerId.ToString() == username));
                if (player == null || player.Player == null)
                    return NotFound();

                var roomsCount = RoomDB.Rooms.Find(r => r.CreatorAccountId == player.PlayerId).Count();
                var inventionsCount = InventionsDB.Inventions.Find(i => i.AccountId == player.PlayerId).Count();
                var friendsCount = 0;

                return Ok(new
                {
                    username = player.Player.Username,
                    displayName = player.Player.DisplayName,
                    accountId = player.PlayerId,
                    roomsCount = roomsCount,
                    inventionsCount = inventionsCount,
                    friendsCount = friendsCount,
                    bio = player.Player.Bio ?? "",
                    createdAt = player.Player.CreatedAt
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("inventions/v2/search")]
        public IActionResult SearchInventions([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                    return Ok(new List<object>());

                var inventions = InventionsDB.Inventions.Find(i => i.Name.Contains(q)).ToList();
                return Ok(inventions.Select(i => new
                {
                    inventionId = i.InventionId,
                    name = i.Name,
                    description = i.Description,
                    imageName = i.ImageName,
                    isPublic = i.IsPublic,
                    createdAt = i.CreatedAt
                }));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("photos/v1/user/{username}")]
        public IActionResult GetUserPhotos(string username)
        {
            return Ok(new List<object>());
        }

        [HttpGet("feed/v1/user/{username}")]
        public IActionResult GetUserFeed(string username)
        {
            return Ok(new List<object>());
        }

        [HttpGet("events/v1/user/{username}")]
        public IActionResult GetUserEvents(string username)
        {
            return Ok(new List<object>());
        }
    }
}