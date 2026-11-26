using System;
using System.Linq;
using System.Text.Json;
using LiteDB;
using Ember2023.Classes;
using Ember2023.Classes.DBs.DBClasses;
using static Ember2023.Classes.DBs.DBClasses.RoomDBClasses;

namespace Ember2023.Classes.DBs
{
    public class RoomDB
    {
        public static LiteDatabase RoomDBFile = new LiteDatabase(Path.Combine(Program.dataDir, "DBs", "Rooms.db"));
        public static readonly ILiteCollection<Room> Rooms = RoomDBFile.GetCollection<Room>("Rooms");
        private static readonly object WriteLock = new();

        public static async Task ImportRooms(string path)
        {
            try
            {
                string jsonData = await File.ReadAllTextAsync(path);
                
                var roomsList = System.Text.Json.JsonSerializer.Deserialize<List<Room>>(jsonData);

                if (roomsList == null) return;

                foreach (var room in roomsList)
                {
                    await AddRoom(room, log: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Import Error] Failed to import JSON: {ex.Message}");
            }
        }

        public static async Task AddRoom(Room newRoom, bool log = false, bool shouldAssignNewIds = true)
        {
            if (newRoom == null) return;

            await Task.Run(() =>
            {
                try
                {
                    if (shouldAssignNewIds || newRoom.RoomId <= 0)
                    {
                        newRoom.RoomId = GetNextRoomId();
                    }

                    newRoom.RankedEntityId = newRoom.RoomId.ToString();

                    if (newRoom.SubRooms != null && newRoom.SubRooms.Any())
                    {
                        long currentMaxSubId = GetNextSubRoomId();
                        foreach (var sub in newRoom.SubRooms)
                        {
                            if (shouldAssignNewIds || sub.SubRoomId <= 0)
                            {
                                sub.SubRoomId = currentMaxSubId;
                                currentMaxSubId++;
                            }
                            sub.RoomId = newRoom.RoomId;
                        }
                    }
                    Rooms.Insert(newRoom);

                    if (log)
                    {
                        Console.WriteLine($"[DB] Added room: {newRoom.Name} (ID: {newRoom.RoomId})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DB Error] Failed to add room {newRoom.Name}: {ex.Message}");
                }
            });
        }

        public static long GetNextRoomId()
        {
            if (Rooms.Count() == 0) 
                return 1;
            
            var maxId = Rooms.Max(x => x.RoomId);

            return Convert.ToInt64(maxId) + 1;
        }

        public static long GetNextSubRoomId()
        {
            var allRooms = Rooms.FindAll().ToList();

            if (allRooms.Count == 0)
                return 1;

            long maxSubId = 0;
            foreach (var room in allRooms)
            {
                if (room.SubRooms != null && room.SubRooms.Any())
                {
                    long currentMax = room.SubRooms.Max(s => Convert.ToInt64(s.SubRoomId));
                    if (currentMax > maxSubId) maxSubId = currentMax;
                }
            }
            return maxSubId + 1;
        }
        
        public static Room GetRoom(long roomId)
        {
            return Rooms.FindById(roomId);
        }

        public static Room GetRoomByName(string name)
        {
            return Rooms.FindOne(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        public static List<Room> GetRoomsByNames(List<string> names)
        {
            if (names == null || names.Count == 0) 
                return new List<Room>();
            
            return Rooms.Find(room => 
                names.Contains(room.Name, StringComparer.OrdinalIgnoreCase)
            ).ToList();
        }
        
        public static void MarkRROsAsCloneable()
        {
            var rros = Rooms.Find(r => r.IsRRO).ToList();
            foreach (var room in rros)
            {
                if (!room.CloningAllowed)
                {
                    room.CloningAllowed = true;
                    Rooms.Update(room);
                }
            }
            Console.WriteLine($"[RoomDB] Marked {rros.Count} RRO rooms as cloneable/templates");
        }

        public static (List<Room> Results, int Total) GetHotRooms(string tag, int skip, int take)
        {
            string? t = tag?.ToLowerInvariant();
            bool isRRO = (t == "rro" || t == "recroomoriginal");

            var candidates = Rooms.FindAll()
                .Where(r => !r.IsDorm)
                .ToList();

            if (isRRO)
            {
                candidates = candidates
                    .Where(r => r.Tags != null && r.Tags.Any(tagRow =>
                        string.Equals(tagRow.Tag, "rro", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(tagRow.Tag, "recroomoriginal", StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            else if (t != "new")
            {
                candidates = candidates
                    .Where(r => r.Tags == null || !r.Tags.Any(tagRow =>
                        string.Equals(tagRow.Tag, "base", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(tagRow.Tag, "rro", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(tagRow.Tag, "recroomoriginal", StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (!isRRO)
            {
                candidates = candidates
                    .Where(r => r.Accessibility == RoomAccessibility.Public)
                    .ToList();
            }

            var ordered = (t == "new")
                ? candidates.OrderByDescending(r => r.CreatedAt)
                : candidates.OrderByDescending(r => r.Stats?.VisitCount ?? 0);

            var results = ordered.Skip(skip).Take(take).ToList();
            int total = candidates.Count;

            return (results, total);
        }
    }
}