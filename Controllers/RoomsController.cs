using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomsController : ControllerBase
    {
        private static readonly LiteDatabase RoomsDb =
            new(Path.Combine(Program.dataDir, "DBs", "EmberRooms.db"));
        private static readonly ILiteCollection<RoomEntry> RoomEntries =
            RoomsDb.GetCollection<RoomEntry>("RoomEntries");

        static RoomsController()
        {
            RoomEntries.EnsureIndex(r => r.AccountId);
            RoomEntries.EnsureIndex(r => r.RoomId);
        }

        public class RoomEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string RoomId { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public bool CloningAllowed { get; set; } = true;
            public bool IsRRO { get; set; } = true;
            public string ImageName { get; set; } = "";
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        [HttpPost("v1/create")]
        public IActionResult CreateRoom([FromBody] CreateRoomRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var room = new RoomEntry
            {
                AccountId = (long)id,
                Name = request.Name ?? "New Room",
                Description = request.Description ?? "",
                CloningAllowed = request.CloningAllowed,
                IsRRO = request.IsRRO,
                ImageName = request.ImageName ?? "",
            };
            RoomEntries.Insert(room);

            return Ok(new
            {
                roomId = room.RoomId,
                name = room.Name,
                description = room.Description,
                cloningAllowed = room.CloningAllowed,
                isRRO = room.IsRRO,
                imageName = room.ImageName,
                createdAt = room.CreatedAt
            });
        }

        [HttpGet("v1/mine")]
        public IActionResult GetMyRooms()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var rooms = RoomEntries.Find(r => r.AccountId == (long)id)
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
                    createdAt = r.CreatedAt
                }).ToList(),
                TotalResults = rooms.Count
            });
        }

        [HttpGet("v1/{roomId}")]
        public IActionResult GetRoom(string roomId)
        {
            var room = RoomEntries.FindOne(r => r.RoomId == roomId);
            if (room == null)
                return NotFound();

            return Ok(new
            {
                roomId = room.RoomId,
                name = room.Name,
                description = room.Description,
                cloningAllowed = room.CloningAllowed,
                isRRO = room.IsRRO,
                imageName = room.ImageName,
                createdAt = room.CreatedAt
            });
        }

        [HttpGet("v1/list")]
        public IActionResult ListRooms()
        {
            var rooms = RoomEntries.FindAll()
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
                    createdAt = r.CreatedAt
                }).ToList(),
                TotalResults = rooms.Count
            });
        }

        [HttpPost("v1/clone/{sourceRoomId}")]
        public IActionResult CloneRoom(string sourceRoomId, [FromBody] CloneRoomRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var sourceRoom = RoomEntries.FindOne(r => r.RoomId == sourceRoomId);
            if (sourceRoom == null)
                return NotFound(new { error = "Source room not found" });

            if (!sourceRoom.CloningAllowed)
                return BadRequest(new { error = "Room cloning is not allowed" });

            var clonedRoom = new RoomEntry
            {
                AccountId = (long)id,
                Name = request.Name ?? $"Copy of {sourceRoom.Name}",
                Description = request.Description ?? sourceRoom.Description,
                CloningAllowed = request.CloningAllowed ?? sourceRoom.CloningAllowed,
                IsRRO = sourceRoom.IsRRO,
                ImageName = sourceRoom.ImageName,
            };
            RoomEntries.Insert(clonedRoom);

            return Ok(new
            {
                roomId = clonedRoom.RoomId,
                name = clonedRoom.Name,
                description = clonedRoom.Description,
                cloningAllowed = clonedRoom.CloningAllowed,
                isRRO = clonedRoom.IsRRO,
                imageName = clonedRoom.ImageName,
                clonedFrom = sourceRoomId,
                createdAt = clonedRoom.CreatedAt
            });
        }

        [HttpGet("v1/templates")]
        public IActionResult GetRoomTemplates()
        {
            var templates = RoomEntries.Find(r => r.CloningAllowed && r.IsRRO)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return Ok(new
            {
                Results = templates.Select(r => new
                {
                    roomId = r.RoomId,
                    name = r.Name,
                    description = r.Description,
                    cloningAllowed = r.CloningAllowed,
                    isRRO = r.IsRRO,
                    imageName = r.ImageName,
                    createdAt = r.CreatedAt
                }).ToList(),
                TotalResults = templates.Count
            });
        }

        public class CreateRoomRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool CloningAllowed { get; set; } = true;
            public bool IsRRO { get; set; } = true;
            public string? ImageName { get; set; }
        }

        public class CloneRoomRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool? CloningAllowed { get; set; }
        }
    }
}
