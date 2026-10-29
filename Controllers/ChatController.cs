using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/chat")]
    public class ChatController : ControllerBase
    {
        private static readonly LiteDatabase ChatDb =
            new(Path.Combine(Program.dataDir, "DBs", "Chat.db"));
        private static readonly ILiteCollection<MessageEntry> Messages =
            ChatDb.GetCollection<MessageEntry>("Messages");

        static ChatController()
        {
            Messages.EnsureIndex(m => m.RoomId);
            Messages.EnsureIndex(m => m.CreatedAt);
        }

        public class MessageEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string RoomId { get; set; } = "";
            public long AccountId { get; set; }
            public string Username { get; set; } = "";
            public string Text { get; set; } = "";
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class SendMessageRequest
        {
            public string RoomId { get; set; } = "";
            public string Text { get; set; } = "";
        }

        [HttpGet("v1/messages/{roomId}")]
        public IActionResult GetMessages(string roomId, [FromQuery] int limit = 50)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var msgs = Messages.Find(m => m.RoomId == roomId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToList()
                .OrderBy(m => m.CreatedAt);

            return Ok(new
            {
                Results = msgs.Select(m => new
                {
                    id = m.Id,
                    roomId = m.RoomId,
                    accountId = m.AccountId,
                    username = m.Username,
                    text = m.Text,
                    createdAt = m.CreatedAt.ToString("O")
                }).ToList(),
                TotalResults = msgs.Count()
            });
        }

        [HttpPost("v1/send")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            var msg = new MessageEntry
            {
                RoomId = request.RoomId ?? "",
                AccountId = (long)id,
                Username = player.Player.Username ?? "Unknown",
                Text = request.Text ?? "",
                CreatedAt = DateTime.UtcNow
            };
            Messages.Insert(msg);

            return Ok(new
            {
                id = msg.Id,
                roomId = msg.RoomId,
                accountId = msg.AccountId,
                username = msg.Username,
                text = msg.Text,
                createdAt = msg.CreatedAt.ToString("O")
            });
        }
    }
}
