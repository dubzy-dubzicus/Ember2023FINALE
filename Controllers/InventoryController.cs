using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/inventory")]
    public class InventoryController : ControllerBase
    {
        [HttpGet("v1/owned")]
        public IActionResult GetOwnedItems()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return Ok(new { Results = new List<object>(), TotalResults = 0 });

            // Return avatar items + any owned inventory items
            var items = new List<object>();
            foreach (var itemId in player.Player.PlayerExtra.AvatarItems)
            {
                items.Add(new { itemId = itemId, type = "avatar_item", owned = true });
            }

            return Ok(new { Results = items, TotalResults = items.Count });
        }

        [HttpPost("v1/equip/{itemId}")]
        public IActionResult EquipItem(string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (!player.Player.PlayerExtra.AvatarItems.Contains(itemId))
            {
                player.Player.PlayerExtra.AvatarItems.Add(itemId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { itemId = itemId, equipped = true });
        }

        [HttpDelete("v1/unequip/{itemId}")]
        public IActionResult UnequipItem(string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            player.Player.PlayerExtra.AvatarItems.Remove(itemId);
            PlayerDB.Players.Update(player);

            return Ok(new { itemId = itemId, equipped = false });
        }

        [HttpPost("v1/consume/{itemId}")]
        public IActionResult ConsumeItem(string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            // Consumable items are removed on use
            if (player.Player.PlayerExtra.AvatarItems.Contains(itemId))
            {
                player.Player.PlayerExtra.AvatarItems.Remove(itemId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { itemId = itemId, consumed = true, success = true });
        }
    }

    [ApiController]
    [Route("/api/store")]
    public class StoreController : ControllerBase
    {
        [HttpGet("v1/items")]
        public IActionResult GetStoreItems()
        {
            return Ok(new
            {
                Results = new List<object>
                {
                    new { itemId = "backpack_unicorn", name = "Unicorn Backpack", price = 500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "paint_pistol_gold", name = "Gold Paint Pistol", price = 1000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_rainbow", name = "Rainbow Paint Pistol", price = 2000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "food_pizza", name = "Pizza", price = 10, currency = "tokens", type = "food" },
                    new { itemId = "food_cake", name = "Cake", price = 25, currency = "tokens", type = "food" },
                    new { itemId = "cloth_suit", name = "Suit", price = 300, currency = "tokens", type = "clothing" },
                },
                TotalResults = 6
            });
        }

        [HttpPost("v1/purchase/{itemId}")]
        public IActionResult PurchaseItem(string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            // Add item to owned items
            if (!player.Player.PlayerExtra.AvatarItems.Contains(itemId))
            {
                player.Player.PlayerExtra.AvatarItems.Add(itemId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { itemId = itemId, purchased = true, success = true });
        }
    }
}
