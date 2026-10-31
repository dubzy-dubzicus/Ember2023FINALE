using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using Ember2023.Classes.DBs.DBClasses;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using PDBC = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;
using RDB = Ember2023.Classes.DBs.DBClasses.RoomDBClasses;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api")]
    public class StockApiController : ControllerBase
    {
        // ============ AVATAR ENDPOINTS ============
        [HttpGet("avatar/v2/set")]
        public IActionResult GetAvatar()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null || player.Player.PlayerExtra == null)
                return NotFound();

            var avatar = player.Player.PlayerExtra.Avatar ?? new PDBC.Avatar();
            return Ok(new
            {
                outfitSelections = avatar.OutfitSelections ?? "",
                faceFeatures = avatar.FaceFeatures ?? "",
                skinColor = avatar.SkinColor ?? "",
                hairColor = avatar.HairColor ?? "",
                avatarItems = player.Player.PlayerExtra.AvatarItems ?? new List<string>(),
                savedAvatars = player.Player.PlayerExtra.SavedAvatars ?? new List<PDBC.SavedOutfit>()
            });
        }

        [HttpPost("avatar/v2/set")]
        public IActionResult SaveAvatar([FromBody] AvatarSaveRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null || player.Player.PlayerExtra == null)
                return NotFound();

            var avatar = player.Player.PlayerExtra.Avatar ?? new PDBC.Avatar();
            if (!string.IsNullOrEmpty(request.OutfitSelections)) avatar.OutfitSelections = request.OutfitSelections;
            if (!string.IsNullOrEmpty(request.FaceFeatures)) avatar.FaceFeatures = request.FaceFeatures;
            if (!string.IsNullOrEmpty(request.SkinColor)) avatar.SkinColor = request.SkinColor;
            if (!string.IsNullOrEmpty(request.HairColor)) avatar.HairColor = request.HairColor;

            player.Player.PlayerExtra.Avatar = avatar;
            PlayerDB.Players.Update(player);
            return NoContent();
        }

        // ============ ACCOUNT ENDPOINTS ============
        [HttpPost("account/account/login")]
        public IActionResult AccountLogin([FromBody] AccountLoginRequest request)
        {
            var account = PlayerDB.Players.Find(x =>
                x.Player != null &&
                x.Player.Username != null &&
                x.Player.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

            if (account == null)
                return BadRequest(new { error = "invalid_credentials" });

            if (string.IsNullOrEmpty(account.Password) ||
                !BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
                return BadRequest(new { error = "invalid_credentials" });

            account.Player.LastLoginAt = DateTime.UtcNow;
            PlayerDB.Players.Update(account);

            string accessToken = AuthStuff.Encode(account.PlayerId);
            return Ok(new
            {
                access_token = accessToken,
                account_id = account.PlayerId,
                username = account.Player.Username,
                display_name = account.Player.DisplayName,
                error = "",
                error_description = ""
            });
        }

        [HttpPost("account/account/register")]
        public IActionResult AccountRegister([FromBody] AccountRegisterRequest request)
        {
            var existing = PlayerDB.Players.FindAll()
                .FirstOrDefault(c => c.Player != null && c.Player.Username == request.Username);
            if (existing != null)
                return BadRequest(new { error = "username_taken" });

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newPlayerData = new PDBC.Player
            {
                Username = request.Username,
                DisplayName = request.DisplayName ?? request.Username,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                ProfileImage = "DefaultPFP.png",
                AvailableUsernameChanges = 3,
                IsJunior = false,
                Level = 1,
                XP = 0,
                PlayerExtra = new PDBC.PlayerExtra
                {
                    Settings = new List<PDBC.Setting>
                    {
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasStarted", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasChosenUsername", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasCreatedPassword", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasFinished", Value = "True" },
                        new PDBC.Setting { Key = "TUTORIAL_COMPLETE_MASK", Value = "57" }
                    }
                }
            };

            var newFullPlayer = new PDBC.FullPlayer
            {
                PlatformIds = new List<PDBC.mPlatformID>
                {
                    new PDBC.mPlatformID { Platform = PDBC.Platforms.Steam, PlatformId = 0 }
                },
                Player = newPlayerData,
                PlayerRoles = new List<PDBC.PlayerRoles> { PDBC.PlayerRoles.Developer },
                AuthToken = Guid.NewGuid().ToString(),
                Password = hash
            };

            PlayerDB.Players.Insert(newFullPlayer);
            string accessToken = AuthStuff.Encode(newFullPlayer.PlayerId);

            return Ok(new
            {
                access_token = accessToken,
                account_id = newFullPlayer.PlayerId,
                username = newFullPlayer.Player.Username,
                display_name = newFullPlayer.Player.DisplayName,
                error = "",
                error_description = ""
            });
        }

        [HttpGet("account/account/me")]
        public IActionResult AccountMe()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(new
            {
                accountId = player.PlayerId,
                username = player.Player.Username,
                displayName = player.Player.DisplayName,
                bio = player.Player.Bio ?? "",
                profileImage = player.Player.ProfileImage ?? "DefaultPFP.png",
                level = player.Player.Level,
                xp = player.Player.XP,
                createdAt = player.Player.CreatedAt,
                lastLoginAt = player.Player.LastLoginAt
            });
        }

        // ============ BIO ENDPOINTS ============
        [HttpGet("bio/v1/{accountId}")]
        public IActionResult GetBio(long accountId)
        {
            var player = PlayerDB.Players.FindById(accountId);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(new { bio = player.Player.Bio ?? "" });
        }

        [HttpPut("bio/v1/{accountId}")]
        public IActionResult SaveBio(long accountId, [FromBody] BioSaveRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            if (id != accountId)
                return Forbid();

            var player = PlayerDB.Players.FindById(accountId);
            if (player == null || player.Player == null)
                return NotFound();

            player.Player.Bio = request.Bio ?? "";
            PlayerDB.Players.Update(player);
            return NoContent();
        }

        // ============ ROOM ENDPOINTS ============
        [HttpGet("room/v2/rooms")]
        public IActionResult GetRooms()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var rooms = RoomDB.Rooms.FindAll()
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return Ok(new
            {
                Results = rooms.Select(r => new
                {
                    roomId = r.RoomId,
                    name = r.Name,
                    description = r.Description,
                    cloningAllowed = r.CloningAllowed,
                    isRRO = r.IsRRO,
                    imageName = r.ImageName,
                    ownerAccountId = r.CreatorAccountId,
                    createdAt = r.CreatedAt
                }).ToList(),
                TotalResults = rooms.Count
            });
        }

        [HttpPost("room/v2/create")]
        public IActionResult CreateRoom([FromBody] RoomCreateRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var room = new RDB.Room
            {
                Name = request.Name ?? "New Room",
                Description = request.Description ?? "",
                CloningAllowed = request.CloningAllowed,
                IsRRO = true,
                ImageName = request.ImageName ?? "",
                CreatorAccountId = (long)id
            };
            RoomDB.Rooms.Insert(room);

            return Ok(new
            {
                roomId = room.RoomId,
                name = room.Name,
                description = room.Description,
                ownerAccountId = (long)id
            });
        }

        [HttpGet("room/v2/{roomId}")]
        public IActionResult GetRoom(long roomId)
        {
            var room = RoomDB.Rooms.FindOne(r => r.RoomId == roomId);
            if (room == null)
                return NotFound();

            return Ok(new
            {
                roomId = room.RoomId,
                name = room.Name,
                description = room.Description,
                imageName = room.ImageName,
                ownerAccountId = room.CreatorAccountId
            });
        }

        // ============ CLUBS ENDPOINTS ============
        [HttpGet("clubs/v1/me")]
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

        // ============ INVENTORY ENDPOINTS ============
        [HttpGet("inventory_stuff/v1/owned")]
        public IActionResult GetInventoryOwned()
        {
            return Ok(new { Results = new List<object>(), TotalResults = 0 });
        }

        // ============ WEARABLES ENDPOINTS ============
        [HttpGet("avatar/v2/wearables")]
        public IActionResult GetWearables()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.Wearables ?? new Dictionary<string, string>());
        }

        [HttpPost("avatar/v2/wearables/{slot}")]
        public IActionResult SetWearable(string slot, [FromBody] WearableRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.Wearables == null)
                player.Player.PlayerExtra.Wearables = new Dictionary<string, string>();

            player.Player.PlayerExtra.Wearables[slot] = request.ItemId;
            PlayerDB.Players.Update(player);

            return NoContent();
        }

        // ============ REQUEST MODELS ============
        public class AvatarSaveRequest
        {
            public string? OutfitSelections { get; set; }
            public string? FaceFeatures { get; set; }
            public string? SkinColor { get; set; }
            public string? HairColor { get; set; }
        }

        public class AccountLoginRequest
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class AccountRegisterRequest
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string? DisplayName { get; set; }
        }

        public class BioSaveRequest
        {
            public string? Bio { get; set; }
        }

        public class RoomCreateRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool CloningAllowed { get; set; } = true;
            public string? ImageName { get; set; }
        }

        public class WearableRequest
        {
            public string ItemId { get; set; } = "";
        }
    }
}
