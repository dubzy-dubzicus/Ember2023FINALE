using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes;

namespace Ember2023.Controllers
{
    /// <summary>
    /// Routes the 2023 client calls while opening the Maker Pen, invention
    /// palette, and equipment/store panels. Response shapes follow the 2023
    /// Mocha reference server and read from existing Ember data files.
    /// </summary>
    [ApiController]
    public class MakerPenCompatibilityController : ControllerBase
    {
        [HttpGet("/api/equipment/v2/getUnlocked")]
        public IActionResult GetUnlockedEquipment()
        {
            long? accountId = AuthStuff.GetPlayerId(Request);
            if (!accountId.HasValue)
                return Unauthorized();

            return Ok(EquipmentInventoryStore.GetOrCreate(accountId.Value));
        }

        [HttpGet("/api/consumables/v2/getUnlocked")]
        public IActionResult GetUnlockedConsumables()
        {
            long? accountId = AuthStuff.GetPlayerId(Request);
            if (!accountId.HasValue)
                return Unauthorized();

            return GetNormalizedConsumables();
        }

        [HttpGet("/api/storefronts/v4/balance/{currencyType:int}")]
        public IActionResult GetStorefrontBalance(int currencyType)
        {
            long? accountId = AuthStuff.GetPlayerId(Request);
            if (!accountId.HasValue)
                return Unauthorized();

            return Ok(StorefrontCatalog.GetBalance(currencyType));
        }

        [HttpGet("/api/storefronts/v3/giftdropstore/{storeId:int}")]
        public IActionResult GetGiftDropStore(int storeId) =>
            Ok(StorefrontCatalog.GetStorefront(storeId));

        [HttpGet("/api/storefronts/v1/adcarouselitems")]
        public IActionResult GetAdCarouselItems() =>
            Ok(StorefrontCatalog.GetAdCarouselItems());

        [HttpGet("/subscription/mine/member")]
        public IActionResult GetMyMembership()
        {
            long? accountId = AuthStuff.GetPlayerId(Request);
            if (!accountId.HasValue)
                return Unauthorized();

            return Ok(Array.Empty<object>());
        }

        [HttpGet("/api/customAvatarItems/v2/fromCreator/{creatorId:long}")]
        public IActionResult GetCustomAvatarItemsFromCreator(long creatorId) =>
            Ok(new
            {
                CreatorId = creatorId,
                Results = Array.Empty<object>(),
                TotalResults = 0
            });

        [HttpGet("/api/customAvatarItems/v1/hot")]
        [HttpGet("/api/customAvatarItems/v1/featured")]
        public IActionResult GetEmptyCustomAvatarItems() =>
            Ok(Array.Empty<object>());

        [HttpGet("/api/rooms/v1/filters")]
        public IActionResult GetRoomFilters() => Ok(new
        {
            PinnedTags = new[] { "rro", "quest", "base" },
            PopularTags = new[] { "rro", "quest", "base" },
            Tags = new[]
            {
                "rro", "quest", "base", "pvp", "game", "hangout",
                "art", "music", "sports", "parkour", "story"
            }
        });

        [HttpGet("/roomserver/rooms/visitedby/me")]
        public IActionResult GetVisitedRooms()
        {
            long? accountId = AuthStuff.GetPlayerId(Request);
            if (!accountId.HasValue)
                return Unauthorized();

            return Ok(new
            {
                Results = Array.Empty<object>(),
                TotalResults = 0
            });
        }

        /// <summary>
        /// Ember's consumables file already contains the owned quantities.
        /// The client expects additional normalized fields (Ids/CreatedAts,
        /// Count/Quantity and ownership flags), so add them without inventing
        /// any items that are not in the data file.
        /// </summary>
        private IActionResult GetNormalizedConsumables()
        {
            string path = Path.Combine(Program.dataDir, "APIS", "Items", "Consumables.json");
            if (!System.IO.File.Exists(path))
                return Ok(Array.Empty<object>());

            try
            {
                JsonNode? root = JsonNode.Parse(System.IO.File.ReadAllText(path));
                if (root is not JsonArray sourceItems)
                    return Ok(Array.Empty<object>());

                var normalizedItems = new JsonArray();

                foreach (JsonNode? node in sourceItems)
                {
                    if (node is not JsonObject source)
                        continue;

                    JsonObject item = source.DeepClone().AsObject();

                    var ids = new List<long>();
                    if (item["Ids"] is JsonArray idsNode)
                    {
                        foreach (JsonNode? idNode in idsNode)
                        {
                            if (long.TryParse(idNode?.ToString() ?? "", out long id))
                                ids.Add(id);
                        }
                    }
                    if (ids.Count == 0)
                        continue;

                    long firstId = ids[0];
                    item["Ids"] = new JsonArray(JsonValue.Create(firstId));

                    string createdAt = "2023-04-06T00:00:00.0000000Z";
                    if (item["CreatedAts"] is JsonArray createdAtsNode)
                    {
                        string? firstCreatedAt = createdAtsNode.FirstOrDefault()?.ToString();
                        if (!string.IsNullOrWhiteSpace(firstCreatedAt))
                            createdAt = firstCreatedAt;
                    }
                    item["CreatedAts"] = new JsonArray(JsonValue.Create(createdAt));

                    string descriptor = item["ConsumableItemDesc"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(descriptor))
                        item["ConsumableItemDesc"] = $"legacy-consumable-{firstId}";

                    string friendlyName = item["FriendlyName"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(friendlyName))
                    {
                        friendlyName = $"Consumable {firstId}";
                        item["FriendlyName"] = friendlyName;
                    }

                    int quantity = 1;
                    if (int.TryParse(item["Count"]?.ToString() ?? "", out int sourceCount) && sourceCount > 0)
                        quantity = sourceCount;

                    item["Count"] = quantity;
                    item["InitialCount"] = quantity;
                    item["Quantity"] = quantity;
                    item["IsActive"] = false;
                    item["ActiveDurationMinutes"] = 0;
                    item["IsTransferable"] = false;
                    item["IsOwned"] = true;
                    item["Owned"] = true;
                    item["Purchased"] = true;

                    if (friendlyName.Contains("Hair Dye", StringComparison.OrdinalIgnoreCase))
                    {
                        item["ConsumableItemType"] = 6;
                        item["ItemType"] = 1;
                    }

                    normalizedItems.Add(item);
                }

                return Content(
                    normalizedItems.ToJsonString(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        WriteIndented = false
                    }),
                    "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONSUMABLE NORMALIZE] Failed: {ex.Message}");
                return Ok(Array.Empty<object>());
            }
        }
    }
}
