using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/ws")]
    public class WebSocketController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, ChatRoomSession> RoomSessions = new();

        [HttpGet("chat/{roomId}")]
        public async Task Get(string roomId)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            long accountId = (long)id;
            var player = PlayerDB.Players.FindById(accountId);
            string username = player?.Player?.Username ?? "Unknown";

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var sessionKey = $"{roomId}:{accountId}";
            var session = new ChatRoomSession
            {
                RoomId = roomId,
                AccountId = accountId,
                Username = username,
                WebSocket = webSocket,
                ConnectedAt = DateTime.UtcNow
            };

            RoomSessions[sessionKey] = session;

            try
            {
                await Echo(webSocket, sessionKey);
            }
            catch
            {
                // Connection closed
            }
            finally
            {
                RoomSessions.TryRemove(sessionKey, out _);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }

        private static async Task Echo(WebSocket webSocket, string sessionKey)
        {
            var buffer = new byte[4096];
            var roomSessions = RoomSessions;

            while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    return;
                }

                var messageText = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Broadcast to all in same room
                var broadcast = $"{{\"type\":\"chat\",\"roomId\":\"\",\"accountId\":0,\"username\":\"\",\"text\":\"\",\"timestamp\":\"\"}}";
                var chatMsg = System.Text.Json.JsonSerializer.Serialize(new
                {
                    type = "chat",
                    roomId = GetRoomId(sessionKey),
                    accountId = GetAccountId(sessionKey),
                    username = GetUsername(sessionKey),
                    text = messageText,
                    timestamp = DateTime.UtcNow.ToString("O")
                });

                var broadcastBytes = System.Text.Encoding.UTF8.GetBytes(chatMsg);
                var tasks = new List<Task>();

                foreach (var kvp in roomSessions)
                {
                    if (kvp.Key.StartsWith(GetRoomId(sessionKey) + ":") && kvp.Key != sessionKey)
                    {
                        if (kvp.Value.WebSocket.State == System.Net.WebSockets.WebSocketState.Open)
                        {
                            tasks.Add(kvp.Value.WebSocket.SendAsync(new ArraySegment<byte>(broadcastBytes), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None));
                        }
                    }
                }

                if (tasks.Count > 0)
                    await Task.WhenAll(tasks);
            }
        }

        private static string GetRoomId(string sessionKey) => sessionKey.Split(':')[0];
        private static long GetAccountId(string sessionKey) => long.Parse(sessionKey.Split(':')[1]);
        private static string GetUsername(string sessionKey) => RoomSessions.TryGetValue(sessionKey, out var s) ? s.Username : "Unknown";
    }

    public class ChatRoomSession
    {
        public string RoomId { get; set; } = "";
        public long AccountId { get; set; }
        public string Username { get; set; } = "";
        public System.Net.WebSockets.WebSocket WebSocket { get; set; } = null!;
        public DateTime ConnectedAt { get; set; }
    }
}
