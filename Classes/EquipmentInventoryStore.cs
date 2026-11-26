using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ember2023.Classes
{
    /// <summary>
    /// 2023-compatible unlocked equipment store.
    ///
    /// The client reads its unlocked tools/skins from the per-account file in
    /// Data/SkinUnlocked/&lt;accountId&gt;/Equipment.json. For accounts that do
    /// not have one yet, seed from the full equipment catalog when available
    /// and fall back to the small default catalog.
    /// </summary>
    public static class EquipmentInventoryStore
    {
        private static readonly object StoreLock = new();
        private static readonly JsonSerializerOptions ReadOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private static readonly JsonSerializerOptions WriteOptions = new()
        {
            WriteIndented = true
        };

        public sealed class EquipmentItem
        {
            [JsonPropertyName("PrefabName")]
            public string PrefabName { get; set; } = "";

            [JsonPropertyName("ModificationGuid")]
            public string ModificationGuid { get; set; } = "";

            [JsonPropertyName("UnlockedLevel")]
            public int UnlockedLevel { get; set; }

            [JsonPropertyName("Favorited")]
            public bool Favorited { get; set; }

            [JsonPropertyName("PlatformMask")]
            public int PlatformMask { get; set; } = -1;

            [JsonPropertyName("FriendlyName")]
            public string FriendlyName { get; set; } = "";

            [JsonPropertyName("Tooltip")]
            public string? Tooltip { get; set; }

            [JsonPropertyName("Rarity")]
            public int Rarity { get; set; }

            [JsonPropertyName("ThumbnailImage")]
            public string ThumbnailImage { get; set; } = "";
        }

        public static string GetPlayerEquipmentPath(long accountId)
        {
            string directory = Path.Combine(Program.dataDir, "SkinUnlocked", accountId.ToString());
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "Equipment.json");
        }

        public static void EnsureInitialized(long accountId)
        {
            lock (StoreLock)
            {
                string playerPath = GetPlayerEquipmentPath(accountId);
                if (File.Exists(playerPath))
                    return;

                string itemsDirectory = Path.Combine(Program.dataDir, "APIS", "Items");
                string fullCatalogPath = Path.Combine(itemsDirectory, "Equipment.json");
                string defaultCatalogPath = Path.Combine(itemsDirectory, "EquipmentDefault.json");
                string catalogPath = File.Exists(fullCatalogPath) ? fullCatalogPath : defaultCatalogPath;

                if (!File.Exists(catalogPath))
                {
                    File.WriteAllText(playerPath, "[]");
                    return;
                }

                try
                {
                    List<EquipmentItem> catalog = JsonSerializer.Deserialize<List<EquipmentItem>>(
                        File.ReadAllText(catalogPath),
                        ReadOptions) ?? new List<EquipmentItem>();

                    File.WriteAllText(
                        playerPath,
                        JsonSerializer.Serialize(catalog, WriteOptions));
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"[EQUIPMENT INVENTORY] Could not seed account {accountId}: {ex.Message}");
                    File.WriteAllText(playerPath, "[]");
                }
            }
        }

        public static List<EquipmentItem> GetOrCreate(long accountId)
        {
            lock (StoreLock)
            {
                EnsureInitialized(accountId);
                string json = File.ReadAllText(GetPlayerEquipmentPath(accountId));
                return JsonSerializer.Deserialize<List<EquipmentItem>>(json, ReadOptions)
                       ?? new List<EquipmentItem>();
            }
        }
    }
}
