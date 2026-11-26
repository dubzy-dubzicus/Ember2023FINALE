using System.Text.Json;

namespace Ember2023.Classes
{
    /// <summary>
    /// 2023 catalog/storefront shapes backed by the existing Ember item files.
    /// No external services and no invented placeholder items.
    /// </summary>
    public static class StorefrontCatalog
    {
        private const int StoreItemPrice = 100;

        private static readonly object CatalogLock = new();
        private static readonly JsonSerializerOptions ReadOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static DateTime AvatarCatalogTimestampUtc;
        private static DateTime EquipmentCatalogTimestampUtc;
        private static DateTime ConsumableCatalogTimestampUtc;
        private static List<CatalogSku> CatalogCache = new();

        public sealed class CatalogPrice
        {
            public int CurrencyType { get; set; }
            public int Price { get; set; }
        }

        public sealed class CatalogGiftDrop
        {
            public int GiftDropId { get; set; }
            public string FriendlyName { get; set; } = "";
            public string Tooltip { get; set; } = "";
            public string AvatarItemDesc { get; set; } = "";
            public string ConsumableItemDesc { get; set; } = "";
            public string EquipmentPrefabName { get; set; } = "";
            public string EquipmentModificationGuid { get; set; } = "";
            public int ItemType { get; set; }
            public int ConsumableItemType { get; set; }
            public int Rarity { get; set; }
            public bool IsQuery { get; set; }
            public bool Unique { get; set; }
            public int Level { get; set; }
            public int Context { get; set; }
            public string ThumbnailImage { get; set; } = "";
        }

        public sealed class CatalogSku
        {
            public long Id { get; set; }
            public long SkuId { get; set; }
            public int PurchasableItemId { get; set; }
            public int Type { get; set; }
            public bool IsFeatured { get; set; }
            public long ItemId { get; set; }
            public long AvatarItemId { get; set; }
            public int ItemType { get; set; }
            public int ConsumableItemType { get; set; }
            public int AvatarItemType { get; set; }
            public string AvatarItemDesc { get; set; } = "";
            public string ConsumableItemDesc { get; set; } = "";
            public string EquipmentPrefabName { get; set; } = "";
            public string EquipmentModificationGuid { get; set; } = "";
            public string FriendlyName { get; set; } = "";
            public string Tooltip { get; set; } = "";
            public string ThumbnailImage { get; set; } = "";
            public int Rarity { get; set; }
            public int PlatformMask { get; set; } = -1;
            public int CurrencyType { get; set; } = 2;
            public int Price { get; set; } = StoreItemPrice;
            public int OriginalPrice { get; set; } = StoreItemPrice;
            public int DiscountedPrice { get; set; } = StoreItemPrice;
            public bool IsAvailable { get; set; } = true;
            public bool IsPurchasable { get; set; } = true;
            public bool IsEnabled { get; set; } = true;
            public bool IsOwned { get; set; }
            public bool Owned { get; set; }
            public bool Purchased { get; set; }
            public bool IsBaseAvatarItem { get; set; }
            public bool HasRealName { get; set; } = true;
            public DateTime CreatedAt { get; set; }
            public List<CatalogPrice> Prices { get; set; } = new();
            public CatalogGiftDrop GiftDrop { get; set; } = new();
            public List<CatalogGiftDrop> GiftDrops { get; set; } = new();
        }

        public sealed class CatalogStorefront
        {
            public int StorefrontType { get; set; }
            public DateTime NextUpdate { get; set; }
            public List<CatalogSku> StoreItems { get; set; } = new();
        }

        public sealed class StorefrontAdCarouselItem
        {
            public int AdCarouselItemId { get; set; }
            public string ImageName { get; set; } = "";
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public List<int> PurchasableItemIds { get; set; } = new();
            public int? PurchaseReminderId { get; set; }
        }

        public sealed class CatalogBalance
        {
            public long Balance { get; set; }
            public int CurrencyType { get; set; }
            public int BalanceType { get; set; }
        }

        private sealed class CatalogSourceItem
        {
            public string? AvatarItemDesc { get; set; }
            public int AvatarItemType { get; set; }
            public int PlatformMask { get; set; } = -1;
            public string? FriendlyName { get; set; }
            public string? Tooltip { get; set; }
            public int Rarity { get; set; }
            public long AvatarItemId { get; set; }
            public bool IsBaseAvatarItem { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? ThumbnailImage { get; set; }
        }

        public static CatalogStorefront GetStorefront(int storeId)
        {
            List<CatalogSku> storeItems = storeId == 3
                ? GetWatchStoreSkusWithHairDyes()
                : GetDailyStoreSkus(storeId);

            return new CatalogStorefront
            {
                StorefrontType = storeId,
                NextUpdate = DateTime.Today.AddDays(1).ToUniversalTime(),
                StoreItems = storeItems
            };
        }

        public static CatalogBalance[] GetBalance(int currencyType) => new[]
        {
            new CatalogBalance
            {
                Balance = 0,
                CurrencyType = currencyType,
                BalanceType = -1
            }
        };

        public static StorefrontAdCarouselItem[] GetAdCarouselItems()
        {
            List<CatalogSku> catalog = GetCatalogSkus()
                .Where(IsUsableStoreItem)
                .ToList();

            List<int> purchasableIds = catalog
                .Select(item => item.PurchasableItemId)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            int avatarItems = catalog.Count(IsAvatarCatalogItem);
            int equipmentSkins = catalog.Count(IsEquipmentSkin);
            int consumables = catalog.Count(IsConsumableCatalogItem);

            return new[]
            {
                new StorefrontAdCarouselItem
                {
                    AdCarouselItemId = 1,
                    ImageName = "DefaultPFP.png",
                    Title = "The All-I-Got Store",
                    Description =
                        $"Everything EMBER has: {avatarItems} avatar items, " +
                        $"{equipmentSkins} skins, and {consumables} consumables. " +
                        $"{StoreItemPrice} tokens each.",
                    PurchasableItemIds = purchasableIds,
                    PurchaseReminderId = null
                }
            };
        }

        private static List<CatalogSku> GetDailyStoreSkus(int storeId) =>
            GetCatalogSkus()
                .Where(IsUsableStoreItem)
                .GroupBy(GetCatalogIdentity, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.FriendlyName, StringComparer.OrdinalIgnoreCase)
                .ToList();

        private static List<CatalogSku> GetWatchStoreSkusWithHairDyes()
        {
            List<CatalogSku> storeItems = GetDailyStoreSkus(3);
            var identities = storeItems
                .Select(GetCatalogIdentity)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            IEnumerable<CatalogSku> hairDyes = GetCatalogSkus()
                .Where(IsHairDyeCatalogItem)
                .GroupBy(GetCatalogIdentity, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.FriendlyName, StringComparer.OrdinalIgnoreCase);

            foreach (CatalogSku hairDye in hairDyes)
            {
                if (identities.Add(GetCatalogIdentity(hairDye)))
                    storeItems.Add(hairDye);
            }

            return storeItems;
        }

        private static List<CatalogSku> GetCatalogSkus()
        {
            string itemsDirectory = Path.Combine(Program.dataDir, "APIS", "Items");
            string avatarPath = Path.Combine(itemsDirectory, "AvatarItems.json");
            string equipmentPath = Path.Combine(itemsDirectory, "Equipment.json");
            string consumablesPath = Path.Combine(itemsDirectory, "Consumables.json");

            bool hasAvatarCatalog = File.Exists(avatarPath);
            bool hasEquipmentCatalog = File.Exists(equipmentPath);
            bool hasConsumableCatalog = File.Exists(consumablesPath);
            if (!hasAvatarCatalog && !hasEquipmentCatalog && !hasConsumableCatalog)
                return new List<CatalogSku>();

            DateTime avatarTimestamp = hasAvatarCatalog ? File.GetLastWriteTimeUtc(avatarPath) : DateTime.MinValue;
            DateTime equipmentTimestamp = hasEquipmentCatalog ? File.GetLastWriteTimeUtc(equipmentPath) : DateTime.MinValue;
            DateTime consumableTimestamp = hasConsumableCatalog ? File.GetLastWriteTimeUtc(consumablesPath) : DateTime.MinValue;

            lock (CatalogLock)
            {
                if (CatalogCache.Count > 0 &&
                    avatarTimestamp == AvatarCatalogTimestampUtc &&
                    equipmentTimestamp == EquipmentCatalogTimestampUtc &&
                    consumableTimestamp == ConsumableCatalogTimestampUtc)
                {
                    return CatalogCache;
                }

                var catalog = new List<CatalogSku>();

                if (hasAvatarCatalog)
                {
                    var avatarItems = JsonSerializer.Deserialize<List<CatalogSourceItem>>(
                        File.ReadAllText(avatarPath), ReadOptions) ?? new List<CatalogSourceItem>();
                    catalog.AddRange(avatarItems.Select(CreateAvatarCatalogSku));
                }

                if (hasEquipmentCatalog)
                {
                    var equipmentItems = JsonSerializer.Deserialize<List<EquipmentInventoryStore.EquipmentItem>>(
                        File.ReadAllText(equipmentPath), ReadOptions)
                        ?? new List<EquipmentInventoryStore.EquipmentItem>();
                    catalog.AddRange(equipmentItems
                        .Where(item => !string.IsNullOrWhiteSpace(item.PrefabName) &&
                                       !string.IsNullOrWhiteSpace(item.ModificationGuid))
                        .Select(CreateEquipmentCatalogSku));
                }

                if (hasConsumableCatalog)
                {
                    using JsonDocument document = JsonDocument.Parse(File.ReadAllText(consumablesPath));
                    IEnumerable<JsonElement> consumables = document.RootElement.ValueKind == JsonValueKind.Array
                        ? document.RootElement.EnumerateArray().Select(item => item.Clone())
                        : Enumerable.Empty<JsonElement>();
                    catalog.AddRange(consumables.Select(CreateConsumableCatalogSku));
                }

                CatalogCache = catalog
                    .Where(IsUsableStoreItem)
                    .GroupBy(GetCatalogIdentity, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .ToList();
                AvatarCatalogTimestampUtc = avatarTimestamp;
                EquipmentCatalogTimestampUtc = equipmentTimestamp;
                ConsumableCatalogTimestampUtc = consumableTimestamp;
                return CatalogCache;
            }
        }

        private static CatalogSku CreateAvatarCatalogSku(CatalogSourceItem item, int index)
        {
            int purchasableItemId = 1_000_000 + index;
            string friendlyName = string.IsNullOrWhiteSpace(item.FriendlyName)
                ? $"Avatar Item {item.AvatarItemId}"
                : item.FriendlyName.Trim();
            CatalogGiftDrop giftDrop = CreateCatalogGiftDrop(item, index, friendlyName);

            return new CatalogSku
            {
                Id = purchasableItemId,
                SkuId = purchasableItemId,
                PurchasableItemId = purchasableItemId,
                Type = 0,
                IsFeatured = true,
                ItemId = item.AvatarItemId,
                AvatarItemId = item.AvatarItemId,
                AvatarItemType = item.AvatarItemType,
                ItemType = item.AvatarItemType,
                AvatarItemDesc = item.AvatarItemDesc ?? "",
                FriendlyName = friendlyName,
                Tooltip = item.Tooltip ?? "",
                ThumbnailImage = item.ThumbnailImage ?? "",
                Rarity = item.Rarity,
                PlatformMask = item.PlatformMask,
                IsBaseAvatarItem = item.IsBaseAvatarItem,
                HasRealName = !string.IsNullOrWhiteSpace(item.FriendlyName),
                CreatedAt = item.CreatedAt,
                Prices = CreateCatalogPrices(),
                GiftDrop = giftDrop,
                GiftDrops = new List<CatalogGiftDrop> { giftDrop }
            };
        }

        private static CatalogSku CreateEquipmentCatalogSku(
            EquipmentInventoryStore.EquipmentItem item,
            int index)
        {
            int purchasableItemId = 2_000_000 + index;
            string friendlyName = GetEquipmentFriendlyName(item, index);
            CatalogGiftDrop giftDrop = CreateCatalogGiftDrop(item, index, friendlyName);

            return new CatalogSku
            {
                Id = purchasableItemId,
                SkuId = purchasableItemId,
                PurchasableItemId = purchasableItemId,
                Type = 0,
                IsFeatured = true,
                ItemId = purchasableItemId,
                ItemType = 0,
                EquipmentPrefabName = item.PrefabName.Trim(),
                EquipmentModificationGuid = item.ModificationGuid.Trim(),
                FriendlyName = friendlyName,
                Tooltip = item.Tooltip ?? "",
                ThumbnailImage = item.ThumbnailImage ?? "",
                Rarity = item.Rarity,
                PlatformMask = item.PlatformMask,
                HasRealName = !string.IsNullOrWhiteSpace(item.FriendlyName),
                Prices = CreateCatalogPrices(),
                GiftDrop = giftDrop,
                GiftDrops = new List<CatalogGiftDrop> { giftDrop }
            };
        }

        private static CatalogSku CreateConsumableCatalogSku(JsonElement item, int index)
        {
            int purchasableItemId = 3_000_000 + index;
            string desc = GetJsonString(item, "ConsumableItemDesc", "ConsumableDesc", "ItemDesc", "DescriptionId");
            if (string.IsNullOrWhiteSpace(desc))
                desc = GetJsonString(item, "FriendlyName", "Name");
            string friendlyName = GetJsonString(item, "FriendlyName", "Name");
            bool hasRealName = !string.IsNullOrWhiteSpace(friendlyName);
            if (string.IsNullOrWhiteSpace(friendlyName))
                friendlyName = $"Consumable {index + 1}";

            bool isHairDye = IsHairDyeFriendlyName(friendlyName);
            int rarity = GetJsonInt(item, "Rarity");
            int itemType = GetJsonInt(item, "ConsumableItemType", "ItemType", "Type");
            if (isHairDye)
                itemType = 1;

            var giftDrop = new CatalogGiftDrop
            {
                GiftDropId = purchasableItemId,
                FriendlyName = friendlyName,
                Tooltip = GetJsonString(item, "Tooltip", "Description"),
                ConsumableItemDesc = desc,
                ItemType = itemType,
                ConsumableItemType = isHairDye ? 6 : itemType,
                Rarity = rarity,
                IsQuery = false,
                Unique = false,
                Level = rarity,
                Context = 100010,
                ThumbnailImage = GetJsonString(item, "ThumbnailImageName", "ThumbnailImage", "Thumbnail", "ImageName")
            };

            return new CatalogSku
            {
                Id = purchasableItemId,
                SkuId = purchasableItemId,
                PurchasableItemId = purchasableItemId,
                Type = 0,
                IsFeatured = true,
                ItemId = GetJsonLong(purchasableItemId, item, "ConsumableItemId", "ItemId", "Id"),
                ItemType = itemType,
                ConsumableItemType = isHairDye ? 6 : itemType,
                ConsumableItemDesc = desc,
                FriendlyName = friendlyName,
                Tooltip = GetJsonString(item, "Tooltip", "Description"),
                ThumbnailImage = giftDrop.ThumbnailImage,
                Rarity = rarity,
                PlatformMask = GetJsonIntOrDefault(item, -1, "PlatformMask"),
                HasRealName = hasRealName,
                IsOwned = isHairDye,
                Owned = isHairDye,
                Purchased = isHairDye,
                Prices = CreateCatalogPrices(),
                GiftDrop = giftDrop,
                GiftDrops = new List<CatalogGiftDrop> { giftDrop }
            };
        }

        private static List<CatalogPrice> CreateCatalogPrices() => new()
        {
            new CatalogPrice
            {
                CurrencyType = 2,
                Price = StoreItemPrice
            }
        };

        private static CatalogGiftDrop CreateCatalogGiftDrop(
            CatalogSourceItem item,
            int index,
            string friendlyName) => new()
        {
            GiftDropId = 1_000_000 + index,
            FriendlyName = friendlyName,
            Tooltip = item.Tooltip ?? "",
            AvatarItemDesc = item.AvatarItemDesc ?? "",
            ConsumableItemDesc = "",
            EquipmentPrefabName = "",
            EquipmentModificationGuid = "",
            Rarity = item.Rarity,
            IsQuery = false,
            Unique = true,
            Level = item.Rarity,
            Context = 100010,
            ThumbnailImage = item.ThumbnailImage ?? ""
        };

        private static CatalogGiftDrop CreateCatalogGiftDrop(
            EquipmentInventoryStore.EquipmentItem item,
            int index,
            string friendlyName) => new()
        {
            GiftDropId = 2_000_000 + index,
            FriendlyName = friendlyName,
            Tooltip = item.Tooltip ?? "",
            AvatarItemDesc = "",
            ConsumableItemDesc = "",
            EquipmentPrefabName = item.PrefabName?.Trim() ?? "",
            EquipmentModificationGuid = item.ModificationGuid?.Trim() ?? "",
            Rarity = item.Rarity,
            IsQuery = false,
            Unique = true,
            Level = item.Rarity,
            Context = 100010,
            ThumbnailImage = item.ThumbnailImage ?? ""
        };

        private static string GetEquipmentFriendlyName(
            EquipmentInventoryStore.EquipmentItem item,
            int index)
        {
            if (!string.IsNullOrWhiteSpace(item.FriendlyName))
                return item.FriendlyName.Trim();

            string prefabName = item.PrefabName?.Trim().Trim('[', ']') ?? "";
            if (string.IsNullOrWhiteSpace(prefabName))
                return $"Equipment Skin {index + 1}";

            return prefabName.EndsWith("Skin", StringComparison.OrdinalIgnoreCase)
                ? prefabName
                : $"{prefabName} Skin";
        }

        private static string GetJsonString(JsonElement item, params string[] names)
        {
            foreach (JsonProperty property in item.EnumerateObject())
            {
                if (!names.Any(name => string.Equals(name, property.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                return property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString() ?? ""
                    : property.Value.ToString();
            }
            return "";
        }

        private static int GetJsonInt(JsonElement item, params string[] names) =>
            GetJsonIntOrDefault(item, 0, names);

        private static int GetJsonIntOrDefault(JsonElement item, int defaultValue, params string[] names)
        {
            foreach (JsonProperty property in item.EnumerateObject())
            {
                if (!names.Any(name => string.Equals(name, property.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (property.Value.TryGetInt32(out int value))
                    return value;
                if (int.TryParse(property.Value.ToString(), out value))
                    return value;
            }
            return defaultValue;
        }

        private static long GetJsonLong(long defaultValue, JsonElement item, params string[] names)
        {
            foreach (JsonProperty property in item.EnumerateObject())
            {
                if (!names.Any(name => string.Equals(name, property.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (property.Value.TryGetInt64(out long value))
                    return value;
                if (long.TryParse(property.Value.ToString(), out value))
                    return value;
            }
            return defaultValue;
        }

        private static bool IsHairDyeFriendlyName(string? friendlyName) =>
            !string.IsNullOrWhiteSpace(friendlyName) &&
            friendlyName.Contains("Hair Dye", StringComparison.OrdinalIgnoreCase);

        private static bool IsHairDyeCatalogItem(CatalogSku? item) =>
            item != null &&
            IsConsumableCatalogItem(item) &&
            IsHairDyeFriendlyName(item.FriendlyName);

        private static bool IsAvatarCatalogItem(CatalogSku? item) =>
            item != null && !string.IsNullOrWhiteSpace(item.AvatarItemDesc);

        private static bool IsEquipmentSkin(CatalogSku? item) =>
            item != null &&
            !string.IsNullOrWhiteSpace(item.EquipmentPrefabName) &&
            !string.IsNullOrWhiteSpace(item.EquipmentModificationGuid);

        private static bool IsConsumableCatalogItem(CatalogSku? item) =>
            item != null && !string.IsNullOrWhiteSpace(item.ConsumableItemDesc);

        private static bool IsExcludedAvatarItemName(string? friendlyName) =>
            !string.IsNullOrWhiteSpace(friendlyName) &&
            (friendlyName.Contains("Custom", StringComparison.OrdinalIgnoreCase) ||
             friendlyName.Contains("Owner", StringComparison.OrdinalIgnoreCase) ||
             friendlyName.Contains("Shirt", StringComparison.OrdinalIgnoreCase));

        private static bool IsUnfinishedCatalogItem(CatalogSku item) =>
            item.Rarity <= 0 && !item.HasRealName;

        private static bool IsUsableStoreItem(CatalogSku? item) =>
            item != null &&
            !string.IsNullOrWhiteSpace(item.FriendlyName) &&
            (IsAvatarCatalogItem(item) || IsEquipmentSkin(item) || IsConsumableCatalogItem(item)) &&
            !(IsAvatarCatalogItem(item) && IsExcludedAvatarItemName(item.FriendlyName)) &&
            !IsUnfinishedCatalogItem(item);

        private static string GetCatalogIdentity(CatalogSku item) =>
            IsEquipmentSkin(item)
                ? $"equipment|{item.EquipmentPrefabName}|{item.EquipmentModificationGuid}"
                : IsConsumableCatalogItem(item)
                    ? $"consumable|{item.ConsumableItemDesc}"
                    : $"avatar|{item.AvatarItemDesc}";
    }
}
