using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class Matchmaking
{
    public static RoomInstance currentInstance = null;
    
    private static string region = "us";
    private static int currentStatusVisibility = 0;
    private static DateTime lastOnline = DateTime.UtcNow;
    
    public static void Init(WebApplication app)
    {
        if (File.Exists("ServerData/LastHeartbeat.json"))
        {
            currentInstance = JsonSerializer.Deserialize<Heartbeat>(File.ReadAllText("ServerData/LastHeartbeat.json")).roomInstance;
        }

        if (File.Exists("ServerData/LastPhotonRegion"))
        {
            region = File.ReadAllText("ServerData/LastPhotonRegion");
        }
        
        app.MapPost("/player/login", () =>
        {
            return Results.Ok();
        });

        app.MapPost("/player/logout", (HttpRequest request) =>
        {
            for (int i = 0; i < Auth.currentUsers.Count; i++)
            {
                if (Auth.currentUsers[i].token == request.Headers.Authorization[0])
                {
                    Auth.currentUsers.RemoveAt(i);
                    break;
                }
            }
            
            return Results.Ok();
        });

        app.MapGet("/player/qos", () =>
        {
            app.Logger.LogWarning("(Matchmaking) /player/qos STUB");

            List<QosEndpoint> qosEndpoints = new();
            
            QosEndpoint qos = new()
            {
                Id = "stub",
                Address = "127.0.0.1"
            };
            
            qosEndpoints.Add(qos);
            
            return qosEndpoints;
        });

        app.MapPost("/player/heartbeat", async (HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();
            string? reportedVersion = form["appVersion"].FirstOrDefault();

            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    if (!string.IsNullOrWhiteSpace(reportedVersion))
                    {
                        user.appVersion = reportedVersion;
                    }

                    user.UpdateHeartbeat();
                    return user.heartbeat;
                }
            }

            return null;
        }).DisableAntiforgery();

        app.MapPut("/player/statusvisibility", ([FromForm] string statusVisibility, HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    user.currentStatusVisibility = int.Parse(statusVisibility);
                }
            }

            return Results.Ok();
        }).DisableAntiforgery();

        app.MapGet("/player", (string id) =>
        {
            List<Heartbeat> players = new();

            foreach (var user in Auth.currentUsers)
            {
                if (user.id == int.Parse(id))
                {
                    players.Add(user.heartbeat);
                }
            }

            return players;
        });

        app.MapPost("/player/exclusivelogin", () =>
        {
            var response = new
            {
                errorCode = 0
            };
            
            return response;
        });

        // Can't get the in (India?) param because of c# being c#
        app.MapPut("/player/photonregionpings", ([FromForm] int ussc, [FromForm] int us, [FromForm] int cae, [FromForm] int usw, [FromForm] int eu, [FromForm] int sa, [FromForm] int ru, [FromForm] int tr, [FromForm] int jp, [FromForm] int rue, [FromForm] int au, [FromForm] int hk, [FromForm] int asia, [FromForm] int za, [FromForm] int kr, [FromForm] int uae) =>
        {
            List<RegionPing> regionPings = new();
            
            regionPings.Add(new("ussc", ussc));
            regionPings.Add(new("us", us));
            regionPings.Add(new("cae", cae));
            regionPings.Add(new("usw", usw));
            regionPings.Add(new("eu", eu));
            regionPings.Add(new("sa", sa));
            regionPings.Add(new("ru", ru));
            regionPings.Add(new("tr", tr));
            regionPings.Add(new("jp", jp));
            regionPings.Add(new("rue", rue));
            regionPings.Add(new("au", au));
            regionPings.Add(new("hk", hk));
            regionPings.Add(new("asia", asia));
            regionPings.Add(new("za", za));
            regionPings.Add(new("kr", kr));
            regionPings.Add(new("uae", uae));

            RegionPing[] regionLowestPing = regionPings.Where((x) => x.Ping == regionPings.Min(y => y.Ping)).ToArray();

            region = regionLowestPing[0].Name;

            return Results.Ok();
        }).DisableAntiforgery();

        app.MapPost("/matchmake/none", () =>
        {
            return Results.NotFound();
        });
        
        app.MapPost("/matchmake/dorm", (HttpRequest request) =>
        {
            long instanceId = Random.Shared.Next(1000, 999999999);
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    currentInstance = new()
                    {
                        isPrivate = true,
                        location = "76d98498-60a1-430c-ab76-b54a29b7a163",
                        maxCapacity = 4,
                        name = "@" + user.account.username + "'s" + " DormRoom",
                        photonRegion = region,
                        photonRegionId = region,
                        photonRoomId = "Dorm-" + instanceId + "-room",
                        roomId = -1,
                        roomInstanceId = instanceId,
                        subRoomId = -1
                    };

                    user.currentInstance = currentInstance;
                    
                    user.UpdateHeartbeat();
                    
                    return Results.Ok(user.heartbeat);
                }
            }

            return Results.NotFound();
        });

        app.MapPost("/matchmake/room/{id}", (HttpRequest request, long id, [FromQuery] long? subRoomId = null) =>
        {
            long instanceId;
            string region;
            if (!string.IsNullOrEmpty(ServerConfig.config.PhotonInstanceId))
            {
                instanceId = long.Parse(ServerConfig.config.PhotonInstanceId);
            }
            else
            {
                instanceId = Random.Shared.Next(1000, 999999999);
            }
            if (!string.IsNullOrEmpty(ServerConfig.config.PhotonRegion))
            {
                region = ServerConfig.config.PhotonRegion;
            }
            else
            {
                region = Matchmaking.region;
            }
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.currentInstance != null)
                {
                    if (user.currentInstance.roomId == id)
                    {
                        instanceId = user.currentInstance.roomInstanceId;
                        region = user.currentInstance.photonRegion;
                    }
                }
            }
            
            string path = "ServerData/Rooms/" + id + "/info.json";

            Room room = null;
            
            for (int i = 0; i < Rooms.roomNamesToIds.names.Count; i++)
            {
                if (Rooms.roomNamesToIds.names[i].id == id)
                {
                    room = JsonSerializer.Deserialize<Room>(File.ReadAllText("ServerData/Rooms/" + Rooms.roomNamesToIds.names[i].name + "/info.json"));
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }
            
            SubRooms subRoom;

            if (subRoomId != null)
            {
                subRoom = room.SubRooms.FirstOrDefault(s => s.SubRoomId == subRoomId.Value);
            }
            else
            {
                subRoom = room.SubRooms?.FirstOrDefault();
            }
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    currentInstance = new()
                    {
                        name = "^" + room.Name,
                        isPrivate = false,
                        location = subRoom.UnitySceneId,
                        maxCapacity = subRoom.MaxPlayers,
                        photonRegion = region,
                        photonRegionId = region,
                        photonRoomId = "Room-" + room.Name + "-room",
                        roomId = room.RoomId,
                        roomInstanceId = instanceId,
                        subRoomId = subRoom.SubRoomId
                    };

                    user.currentInstance = currentInstance;
                    
                    user.UpdateHeartbeat();
                    
                    return Results.Ok(user.heartbeat);
                }
            }

            return Results.NotFound();
        });

        app.MapPost("/matchmake/room/{roomId}/{subRoomId}", (long roomId, long subRoomId, HttpRequest request) =>
        {
            long instanceId;
            string region;
            if (!string.IsNullOrEmpty(ServerConfig.config.PhotonInstanceId))
            {
                instanceId = long.Parse(ServerConfig.config.PhotonInstanceId);
            }
            else
            {
                instanceId = Random.Shared.Next(1000, 999999999);
            }
            if (!string.IsNullOrEmpty(ServerConfig.config.PhotonRegion))
            {
                region = ServerConfig.config.PhotonRegion;
            }
            else
            {
                region = Matchmaking.region;
            }
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.currentInstance != null)
                {
                    if (user.currentInstance.roomId == roomId)
                    {
                        instanceId = user.currentInstance.roomInstanceId;
                        region = user.currentInstance.photonRegion;
                    }
                }
            }
            
            string path = "ServerData/Rooms/" + roomId + "/info.json";

            Room room = null;
            
            for (int i = 0; i < Rooms.roomNamesToIds.names.Count; i++)
            {
                if (Rooms.roomNamesToIds.names[i].id == roomId)
                {
                    room = JsonSerializer.Deserialize<Room>(File.ReadAllText("ServerData/Rooms/" + Rooms.roomNamesToIds.names[i].name + "/info.json"));
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }
            
            SubRooms subRoom;

            if (subRoomId != null)
            {
                subRoom = room.SubRooms.FirstOrDefault(s => s.SubRoomId == subRoomId);
            }
            else
            {
                subRoom = room.SubRooms?.FirstOrDefault();
            }
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    currentInstance = new()
                    {
                        name = "^" + room.Name,
                        isPrivate = false,
                        location = subRoom.UnitySceneId,
                        maxCapacity = subRoom.MaxPlayers,
                        photonRegion = region,
                        photonRegionId = region,
                        photonRoomId = "Room-" + room.Name + "-room",
                        roomId = room.RoomId,
                        roomInstanceId = instanceId,
                        subRoomId = subRoom.SubRoomId
                    };

                    user.currentInstance = currentInstance;
                    
                    user.UpdateHeartbeat();
                    
                    return Results.Ok(user.heartbeat);
                }
            }

            return Results.NotFound();
        });

        // 2020 cv2 update
        app.MapPost("/goto/room/{roomName}", (string roomName, HttpRequest request) =>
        {
            long instanceId = Random.Shared.Next(1000, 999999999);
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    currentInstance = new()
                    {
                        isPrivate = true,
                        location = "76d98498-60a1-430c-ab76-b54a29b7a163",
                        maxCapacity = 4,
                        name = "@" + user.account.username + "'s" + " DormRoom",
                        photonRegion = region,
                        photonRegionId = region,
                        photonRoomId = "Dorm-" + instanceId + "-room",
                        roomId = -1,
                        roomInstanceId = instanceId,
                        subRoomId = -1
                    };

                    user.currentInstance = currentInstance;
                    
                    user.UpdateHeartbeat();
                    
                    return Results.Ok(user.heartbeat);
                }
            }

            return Results.NotFound();
        });

        app.MapGet("/player/avoidjuniors", () =>
        {
            return bool.Parse(File.ReadAllText("ServerData/Accounts/2/avoidjuniors"));
        });

        app.MapPut("/player/avoidjuniors", ([FromForm] bool avoidJuniors, HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    File.WriteAllText("ServerData/Accounts/" + user.id + "/avoidjuniors", avoidJuniors.ToString());
                    
                    return Results.Ok();
                }
            }

            return Results.NotFound();
            
        }).DisableAntiforgery();

        app.MapGet("/rooms/requiring/rrplus", () => "[]");
        app.MapGet("/rooms/requiring/developer", () => "[]");

        // Some 2023 clients prefix these with /match.
        app.MapGet("/match/rooms/requiring/rrplus", () => "[]");
        app.MapGet("/match/rooms/requiring/developer", () => "[]");

        app.MapPost("/roominstance/{instance}/reportjoinresult", () =>
        {
            return Results.Ok();
        });
    }
}