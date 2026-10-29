using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomSettingsController : ControllerBase
    {
        private static readonly LiteDatabase RoomSettingsDb =
            new(Path.Combine(Program.dataDir, "DBs", "RoomSettings.db"));
        private static readonly ILiteCollection<RoomSettingEntry> RoomSettings =
            RoomSettingsDb.GetCollection<RoomSettingEntry>("RoomSettings");

        static RoomSettingsController()
        {
            RoomSettings.EnsureIndex(r => r.RoomId);
        }

        public class RoomSettingEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string RoomId { get; set; } = "";
            public long OwnerAccountId { get; set; }
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public bool IsPublic { get; set; } = true;
            public bool CloningAllowed { get; set; } = true;
            public bool IsRRO { get; set; } = true;
            public int MaxPlayers { get; set; } = 16;
            public string PrivacyLevel { get; set; } = "public"; // public, friends_only, invite_only, private
            public string ImageName { get; set; } = "";
            public List<string> BannedAccountIds { get; set; } = new();
            public List<string> WhitelistedAccountIds { get; set; } = new();
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }

        [HttpGet("v1/{roomId}/settings")]
        public IActionResult GetRoomSettings(string roomId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null)
                return NotFound();

            return Ok(new
            {
                roomId = settings.RoomId,
                ownerAccountId = settings.OwnerAccountId,
                name = settings.Name,
                description = settings.Description,
                isPublic = settings.IsPublic,
                cloningAllowed = settings.CloningAllowed,
                isRRO = settings.IsRRO,
                maxPlayers = settings.MaxPlayers,
                privacyLevel = settings.PrivacyLevel,
                imageName = settings.ImageName,
                bannedCount = settings.BannedAccountIds.Count,
                whitelistedCount = settings.WhitelistedAccountIds.Count,
                createdAt = settings.CreatedAt.ToString("O"),
                updatedAt = settings.UpdatedAt.ToString("O")
            });
        }

        [HttpPut("v1/{roomId}/settings")]
        public IActionResult UpdateRoomSettings(string roomId, [FromBody] UpdateRoomSettingsRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null)
                return NotFound();

            if (settings.OwnerAccountId != (long)id)
                return Forbid();

            if (request.Name != null) settings.Name = request.Name;
            if (request.Description != null) settings.Description = request.Description;
            if (request.IsPublic.HasValue) settings.IsPublic = request.IsPublic.Value;
            if (request.CloningAllowed.HasValue) settings.CloningAllowed = request.CloningAllowed.Value;
            if (request.IsRRO.HasValue) settings.IsRRO = request.IsRRO.Value;
            if (request.MaxPlayers.HasValue) settings.MaxPlayers = request.MaxPlayers.Value;
            if (request.PrivacyLevel != null) settings.PrivacyLevel = request.PrivacyLevel;
            if (request.ImageName != null) settings.ImageName = request.ImageName;

            settings.UpdatedAt = DateTime.UtcNow;
            RoomSettings.Update(settings);

            return Ok(new { success = true });
        }

        [HttpPost("v1/{roomId}/ban/{targetAccountId}")]
        public IActionResult BanFromRoom(string roomId, long targetAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null || settings.OwnerAccountId != (long)id)
                return Forbid();

            if (!settings.BannedAccountIds.Contains(targetAccountId.ToString()))
                settings.BannedAccountIds.Add(targetAccountId.ToString());

            settings.UpdatedAt = DateTime.UtcNow;
            RoomSettings.Update(settings);

            return Ok(new { success = true, bannedAccountId = targetAccountId });
        }

        [HttpDelete("v1/{roomId}/ban/{targetAccountId}")]
        public IActionResult UnbanFromRoom(string roomId, long targetAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null || settings.OwnerAccountId != (long)id)
                return Forbid();

            settings.BannedAccountIds.Remove(targetAccountId.ToString());
            settings.UpdatedAt = DateTime.UtcNow;
            RoomSettings.Update(settings);

            return Ok(new { success = true, unbannedAccountId = targetAccountId });
        }

        [HttpGet("v1/{roomId}/isBanned/{targetAccountId}")]
        public IActionResult IsBannedFromRoom(string roomId, long targetAccountId)
        {
            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null)
                return Ok(new { isBanned = false });

            return Ok(new { isBanned = settings.BannedAccountIds.Contains(targetAccountId.ToString()) });
        }

        [HttpPost("v1/{roomId}/image")]
        public IActionResult UploadRoomImage(string roomId, [FromForm] UploadRoomImageRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null || settings.OwnerAccountId != (long)id)
                return Forbid();

            if (request.ImageFile == null || request.ImageFile.Length == 0)
                return BadRequest(new { error = "No image file provided" });

            string roomImagesDir = Path.Combine(Program.dataDir, "Images", "Rooms");
            Directory.CreateDirectory(roomImagesDir);

            string fileName = $"room_{roomId}_{Guid.NewGuid().ToString("N")}.png";
            string filePath = Path.Combine(roomImagesDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                request.ImageFile.CopyTo(stream);
            }

            settings.ImageName = fileName;
            settings.UpdatedAt = DateTime.UtcNow;
            RoomSettings.Update(settings);

            return Ok(new
            {
                success = true,
                imageName = fileName,
                imageUrl = $"/imageserver/Rooms/{fileName}"
            });
        }

        [HttpPost("v1/{roomId}/whitelist/{targetAccountId}")]
        public IActionResult WhitelistToRoom(string roomId, long targetAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var settings = RoomSettings.FindOne(r => r.RoomId == roomId);
            if (settings == null || settings.OwnerAccountId != (long)id)
                return Forbid();

            if (!settings.WhitelistedAccountIds.Contains(targetAccountId.ToString()))
                settings.WhitelistedAccountIds.Add(targetAccountId.ToString());

            settings.UpdatedAt = DateTime.UtcNow;
            RoomSettings.Update(settings);

            return Ok(new { success = true, whitelistedAccountId = targetAccountId });
        }

        public class UpdateRoomSettingsRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool? IsPublic { get; set; }
            public bool? CloningAllowed { get; set; }
            public bool? IsRRO { get; set; }
            public int? MaxPlayers { get; set; }
            public string? PrivacyLevel { get; set; }
            public string? ImageName { get; set; }
        }

        public class UploadRoomImageRequest
        {
            public IFormFile ImageFile { get; set; } = null!;
        }
    }
}
