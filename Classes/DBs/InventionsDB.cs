using LiteDB;
using System;
using System.Text.Json;
using Ember2023.Classes;

namespace Ember2023.Classes.DBs
{
    public class InventionsDB
    {
        private static readonly LiteDatabase Database =
            new(Path.Combine(Program.dataDir, "DBs", "Inventions.db"));

        public static readonly ILiteCollection<Invention> Inventions =
            Database.GetCollection<Invention>("Inventions");

        static InventionsDB()
        {
            Inventions.EnsureIndex(inv => inv.AccountId);
            Inventions.EnsureIndex(inv => inv.InventionId);
        }

        public class Invention
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public string InventionId { get; set; } = Guid.NewGuid().ToString("N");
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string BlueprintJson { get; set; } = "{}";
            public string ImageName { get; set; } = "";
            public int Version { get; set; } = 1;
            public bool IsPublic { get; set; } = false;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }

        public static Invention GetInvention(string inventionId)
        {
            return Inventions.FindOne(inv => inv.InventionId == inventionId);
        }

        public static Invention GetInventionById(long accountId, string inventionId)
        {
            return Inventions.FindOne(inv => inv.AccountId == accountId && inv.InventionId == inventionId);
        }

        public static List<Invention> GetInventionsByAccount(long accountId)
        {
            return Inventions.Find(inv => inv.AccountId == accountId)
                .OrderByDescending(inv => inv.UpdatedAt)
                .ToList();
        }

        public static Invention SaveInvention(long accountId, string name, string description, string blueprintJson, string imageName, bool isPublic)
        {
            var existing = Inventions.FindOne(inv => inv.AccountId == accountId && inv.Name == name);
            if (existing != null)
            {
                existing.Name = name;
                existing.Description = description;
                existing.BlueprintJson = blueprintJson;
                existing.ImageName = imageName;
                existing.IsPublic = isPublic;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.Version++;
                Inventions.Update(existing);
                return existing;
            }

            var invention = new Invention
            {
                AccountId = accountId,
                Name = name,
                Description = description,
                BlueprintJson = blueprintJson,
                ImageName = imageName,
                IsPublic = isPublic
            };
            Inventions.Insert(invention);
            return invention;
        }

        public static bool DeleteInvention(long accountId, string inventionId)
        {
            return Inventions.DeleteMany(inv => inv.AccountId == accountId && inv.InventionId == inventionId) > 0;
        }
    }
}
