using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class Rooms
{
    public static NameToIds roomNamesToIds;

    private static void UpdateRoomNamesToIds()
    {
        NameToIds newRoomNamesToIds = new();
        newRoomNamesToIds.names = new();
        
        string[] dirs = Directory.GetDirectories("ServerData/Rooms");
        
        newRoomNamesToIds.count = dirs.Length;

        foreach (var room in dirs)
        {
            Room roomJson = JsonSerializer.Deserialize<Room>(File.ReadAllText(room + "/info.json"));
                
            newRoomNamesToIds.names.Add(new()
            {
                id = roomJson.RoomId,
                name = roomJson.Name
            });
        }
        
        File.WriteAllText("ServerData/RoomNamesToIds.json", JsonSerializer.Serialize(newRoomNamesToIds));
    }
    
    // Every room read must carry the beta tag; the 2023 client uses it to
    // unlock Maker Pen tools/skins. Keep it idempotent for older room files.
    private static Room LoadRoom(string path)
    {
        Room room = JsonSerializer.Deserialize<Room>(File.ReadAllText(path))
            ?? throw new InvalidOperationException("Empty room file: " + path);

        room.Tags ??= new();

        Tags? beta = room.Tags.FirstOrDefault(t => string.Equals(t.Tag, "beta", StringComparison.OrdinalIgnoreCase));
        if (beta == null)
        {
            room.Tags.Insert(0, new("beta", 1));
        }
        else
        {
            beta.Type = 1;
        }

        return room;
    }

    public static void Init(WebApplication app)
    {
        if (!File.Exists("ServerData/RoomNamesToIds.json"))
        {
            UpdateRoomNamesToIds();
        }
        
        roomNamesToIds = JsonSerializer.Deserialize<NameToIds>(File.ReadAllText("ServerData/RoomNamesToIds.json"));

        if (roomNamesToIds.count != Directory.GetDirectories("ServerData/Rooms").Length)
        {
            UpdateRoomNamesToIds();
        }
        
        app.MapGet("/rooms/ownedby/me", () =>
        {
            List<RoomDTO> ownedRooms = new();
            
            DirectoryInfo roomsDir = new("ServerData/Rooms");
            foreach (DirectoryInfo roomDir in roomsDir.EnumerateDirectories())
            {
                RoomDTO room = JsonSerializer.Deserialize<RoomDTO>(File.ReadAllText(roomDir.FullName + "/info.json"));
                ownedRooms.Add(room);
            }
            
            return ownedRooms;
        });

        app.MapGet("/rooms/createdby/me", () =>
        {
            List<RoomDTO> ownedRooms = new();
            
            DirectoryInfo roomsDir = new("ServerData/Rooms");
            foreach (DirectoryInfo roomDir in roomsDir.EnumerateDirectories())
            {
                RoomDTO room = JsonSerializer.Deserialize<RoomDTO>(File.ReadAllText(roomDir.FullName + "/info.json"));
                ownedRooms.Add(room);
            }
            
            return ownedRooms;
        });

        app.MapGet("/rooms/visitedby/me", (int skip, int take) =>
        {
            List<RoomDTO> ownedRooms = new();
            
            DirectoryInfo roomsDir = new("ServerData/Rooms");
            foreach (DirectoryInfo roomDir in roomsDir.EnumerateDirectories())
            {
                RoomDTO room = JsonSerializer.Deserialize<RoomDTO>(File.ReadAllText(roomDir.FullName + "/info.json"));
                ownedRooms.Add(room);
            }
            
            return ownedRooms;
        });

        app.MapPost("/rooms/create", async (HttpRequest request) =>
        {
            var a = await request.ReadFormAsync();
            var name = a.ElementAt(0);
            var ugcVersion = a.ElementAt(1);
            
            app.Logger.LogWarning($"(Rooms) /rooms/create STUB\n{name}\n{ugcVersion}");

            RoomDTO room = new()
            {
                Name = name.Value,
                UgcVersion = byte.Parse(ugcVersion.Value),
                CreatedAt = DateTime.Now
            };
            
            CreateFolder("ServerData/Rooms/" + name.Value);
            
            File.WriteAllText("ServerData/Rooms/" + name.Value + "/info.json", JsonSerializer.Serialize(room));

            return Results.Created("/rooms/create", room);
        });
        
        app.MapPost("/rooms/{id}/clone", (int id, [FromForm] string name, HttpRequest request) =>
        {
            LoggedInUser user = null;
            
            foreach (var user2 in Auth.currentUsers)
            {
                if (user2.token == request.Headers.Authorization[0])
                {
                    user = user2;
                }
            }

            if (user == null)
            {
                return Results.NotFound();
            }
            
            Room room = null;
            
            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == id)
                {
                    room = LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json");
                }
            }

            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].name == name)
                {
                    ApiResponse alreadyExistsResponse = new()
                    {
                        success = false,
                        error = "The room " + roomNamesToIds.names[i].name + " already exists."
                    };

                    return Results.Ok(alreadyExistsResponse);
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }

            List<Tags> tags = new();
            tags.Add(new("beta", 1));
            tags.Add(new("community", 2));
            tags.Add(new("screen", 2));
            tags.Add(new("walkvr", 2));
            tags.Add(new("teleportvr", 2));
            tags.Add(new("junior", 2));
            tags.Add(new("pickout", 2));

            room.Name = name;
            room.RoomId = roomNamesToIds.count + 5;
            room.SubRooms[0].RoomId = room.RoomId;
            room.SubRooms[0].SubRoomId = 0;
            room.SubRooms[0].CreatorAccountId = user.id;
            room.Tags = tags;
            room.ImageName = "lemres.png";
            room.CreatorAccountId = user.id;
            room.Roles[1].AccountId = user.id;
            
            Directory.CreateDirectory("ServerData/Rooms/" + name);
            
            File.WriteAllText("ServerData/Rooms/" + name + "/info.json", JsonSerializer.Serialize(room));
            
            UpdateRoomNamesToIds();
            roomNamesToIds = JsonSerializer.Deserialize<NameToIds>(File.ReadAllText("ServerData/RoomNamesToIds.json"));

            ApiResponse response = new()
            {
                success = true,
                error = string.Empty,
                value = room
            };

            return Results.Ok(response);
        }).DisableAntiforgery();

        app.MapGet("/rooms", (string name, int include) =>
        {
            string path = "ServerData/Rooms/" + name + "/info.json";
            if (!File.Exists(path))
            {
                return Results.NotFound();
            }

            return Results.Ok(LoadRoom(path));
        });

        app.MapGet("/rooms/bulk", (string name, bool? excludePrivateRooms) =>
        {
            if (!Directory.Exists("ServerData/Rooms/" + name))
            {
                return Results.NotFound();
            }

            List<Room> rooms = new();
            
            rooms.Add(LoadRoom("ServerData/Rooms/" + name + "/info.json"));
            
            return Results.Ok(rooms);
        });

        app.MapGet("/rooms/{id}", (int id, int include) =>
        {
            if (File.Exists("ServerData/Rooms/" + id + "/info.json"))
            {
                return Results.Ok(LoadRoom("ServerData/Rooms/" + id + "/info.json"));
            }

            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == id)
                {
                    return Results.Ok(LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json"));
                }
            }

            return Results.NotFound();
        });

        app.MapGet("/rooms/hot", (int skip, int take) =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/hot STUB");
            
            List<Room> rooms = new();
            rooms.Add(LoadRoom("ServerData/Rooms/RecCenter/info.json"));
            
            var pagedRooms = new
            {
                Results = rooms,
                TotalResults = 1
            };

            return pagedRooms;
        });

        app.MapGet("/rooms/{id}/interactionby/me", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{id}}/interactionby/me STUB");

            var result = new
            {
                Cheered = true,
                Favorited = true,
                LastVisitedAt = DateTime.UtcNow
            };

            return result;
        });

        app.MapPut("/rooms/{id}/interactionby/me/favorite", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{id}}/interactionby/me/favorite STUB");

            return new { Favorited = true };
        });

        app.MapDelete("/rooms/{id}/interactionby/me/favorite", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{id}}/interactionby/me/favorite STUB");

            return new { Favorited = false };
        });

        app.MapPut("/rooms/{id}/interactionby/me/cheer", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{id}}/interactionby/me/cheer STUB");

            return new { Cheered = true };
        });

        app.MapDelete("/rooms/{id}/interactionby/me/cheer", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{id}}/interactionby/me/cheer STUB");

            return new { Cheered = false };
        });

        app.MapGet("/photon_access_token", () =>
        {
            string[] granted = new[]
            {
                "CAN_USE_ROOM_RESET_BUTTON",
                "CAN_USE_DELETE_ALL_BUTTON",
                "CAN_SAVE_INVENTIONS",
                "CAN_SPAWN_INVENTIONS",
                "CAN_USE_PLAY_GIZMOS_TOGGLE",
                "CAN_USE_MAKER_PEN"
            };

            PhotonAccessTokenObject accessToken = new()
            {
                Permissions = granted.Select(permission => new PhotonPermission
                {
                    Permission = permission,
                    Role = 0,
                    Override = true
                }).ToArray(),
                PhotonAccessToken = "",
                RoomInstanceId = Matchmaking.currentInstance?.roomInstanceId ?? 0
            };

            return accessToken;
        });

        app.MapGet("/publishState/configs", () =>
        {
            app.Logger.LogWarning("(Rooms) /publishState/configs STUB");

            RoomPublishStateConfig config = new()
            {
                UpdateMaxCount = 5,
                UpdateRollingWindowInDays = 10,
                UpdateExpirationInDays = 9,
                UpdateCooldownInDays = 1
            };

            ApiResponse response = new()
            {
                success = true,
                value = config
            };
            
            return response;
        });

        app.MapGet("/rooms/{roomId}/subrooms/{subroomId}/saves", (int roomId, int subroomId) =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/id/subrooms/id/saves STUB");

            Room room = null;
            
            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == roomId)
                {
                    room = LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json");
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }

            SubRooms subRoom = room.SubRooms[0];

            List<CurrentSave> saves = new();

            if (subRoom.CurrentSave == null)
            {
                saves.Add(new()
                {
                    SubRoomId = (int)room.SubRooms[0].SubRoomId,
                    SubRoomDataSaveId = 1001,
                    ReferencedUnityAssetIds = Array.Empty<object>(),
                    PersistenceVersion = 50,
                    OMVersion = 0,
                    CreatedAt = DateTime.Now,
                    UgcSubVersion = 0,
                });
            }
            else
            {
                saves.Add(subRoom.CurrentSave);
            }

            return Results.Ok(saves[0]);
        });

        app.MapPost("/rooms/{roomId}/subrooms/{subroomId}/data", async (int roomId, int subroomId, HttpRequest request) =>
        {
            Room room = null;
            
            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == roomId)
                {
                    room = LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json");
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }
            
            var body = new StreamReader(request.Body);
            var requestBody = await body.ReadToEndAsync();
            
            SaveRequest saveRequest = JsonSerializer.Deserialize<SaveRequest>(requestBody);

            if (room.SubRooms[0].CurrentSave == null)
            {
                room.SubRooms[0].CurrentSave = new()
                {
                    SubRoomId = (int)room.SubRooms[0].SubRoomId,
                    SubRoomDataSaveId = 1001,
                    ReferencedUnityAssetIds = Array.Empty<object>(),
                    PersistenceVersion = 50,
                    OMVersion = 0,
                    CreatedAt = DateTime.Now,
                    UgcSubVersion = 0,
                };
            }

            room.SubRooms[0].CurrentSave.DataBlob = saveRequest.SubRoomData.Filename;
            room.SubRooms[0].CurrentSave.PersistenceVersion = saveRequest.PersistenceVersion;
            
            ApiResponse response = new()
            {
                error = string.Empty,
                success = true,
                value = new SubroomSaveResponse
                {
                    room = room,
                    subRoomDataSave = room.SubRooms[0].CurrentSave
                }
            };

            if (File.Exists("ServerData/Rooms/" + room.Name + "/info.json.bak"))
            {
                File.Delete("ServerData/Rooms/" + room.Name + "/info.json.bak");
            }
            
            File.Move("ServerData/Rooms/" + room.Name + "/info.json", "ServerData/Rooms/" + room.Name + "/info.json.bak");
            
            await File.WriteAllTextAsync("ServerData/Rooms/" + room.Name + "/info.json", JsonSerializer.Serialize(room));

            return Results.Ok(response);
        }).DisableAntiforgery();
        
        app.MapGet("/rooms/{roomId}/subrooms/{subroomId}/data", (int roomId, int subroomId) =>
        {
            Room room = null;
            
            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == roomId)
                {
                    room = LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json");
                }
            }

            if (room == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(room.SubRooms[0]);
        }).DisableAntiforgery();

        app.MapPost("/rooms/regennamestoids", () =>
        {
            UpdateRoomNamesToIds();
            
            roomNamesToIds = JsonSerializer.Deserialize<NameToIds>(File.ReadAllText("ServerData/RoomNamesToIds.json"));
        });

        app.MapGet("/rooms/{roomId}/playerdata/me", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{roomId}}/playerdata/me STUB");

            RoomPlayerData playerData = new()
            {
                Data = string.Empty
            };

            return playerData;
        });
        
        app.MapGet("/room/{name}", async (string name, HttpContext context) =>
        {
            return Results.File(await File.ReadAllBytesAsync("wwwroot/room/" + name));
        });

        app.MapGet("/rooms/{roomId}/similar", (int roomId) =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/{{roomId}}/similar STUB");

            List<Room> rooms = new();
            rooms.Add(LoadRoom("ServerData/Rooms/RecCenter/info.json"));
            
            var pagedRooms = new
            {
                Results = rooms,
                TotalResults = 1
            };

            return pagedRooms;
        });

        app.MapGet("/rooms/base", () =>
        {
            app.Logger.LogWarning("(Rooms) /rooms/base STUB");
            
            List<Room> rooms = new();
            rooms.Add(LoadRoom("ServerData/Rooms/MakerRoom/info.json"));

            return rooms;
        });

        app.MapGet("/room/{roomId}/instances", (int roomId) =>
        {
            List<RoomInstance> instances = new();
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.currentInstance != null)
                {
                    if (user.currentInstance.roomId == roomId)
                    {
                        instances.Add(user.currentInstance);
                    }
                }
            }

            return instances;
        });

        // 2020
        app.MapGet("/api/rooms/v4/details/{roomId}", (int roomId) =>
        {
            if (File.Exists("ServerData/Rooms/" + roomId + "/info.json"))
            {
                return Results.Ok(LoadRoom("ServerData/Rooms/" + roomId + "/info.json"));
            }

            for (int i = 0; i < roomNamesToIds.names.Count; i++)
            {
                if (roomNamesToIds.names[i].id == roomId)
                {
                    return Results.Ok(LoadRoom("ServerData/Rooms/" + roomNamesToIds.names[i].name + "/info.json"));
                }
            }

            return Results.NotFound();
        });
    }
    
    private static void CreateFolder(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}