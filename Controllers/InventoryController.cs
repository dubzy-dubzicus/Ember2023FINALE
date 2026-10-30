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

        [HttpPost("v1/wearable/equip/{slot}/{itemId}")]
        public IActionResult EquipWearable(string slot, string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.Wearables == null)
                player.Player.PlayerExtra.Wearables = new Dictionary<string, string>();

            player.Player.PlayerExtra.Wearables[slot] = itemId;
            PlayerDB.Players.Update(player);

            return Ok(new { slot = slot, itemId = itemId, equipped = true });
        }

        [HttpDelete("v1/wearable/unequip/{slot}")]
        public IActionResult UnequipWearable(string slot)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.Wearables != null)
                player.Player.PlayerExtra.Wearables.Remove(slot);

            PlayerDB.Players.Update(player);

            return Ok(new { slot = slot, equipped = false });
        }

        [HttpGet("v1/wearables")]
        public IActionResult GetWearables()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.Wearables ?? new Dictionary<string, string>());
        }

        [HttpPost("v1/emote/equip/{emoteId}")]
        public IActionResult EquipEmote(string emoteId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (!player.Player.PlayerExtra.EquippedEmotes.Contains(emoteId))
            {
                player.Player.PlayerExtra.EquippedEmotes.Add(emoteId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { emoteId = emoteId, equipped = true });
        }

        [HttpDelete("v1/emote/unequip/{emoteId}")]
        public IActionResult UnequipEmote(string emoteId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            player.Player.PlayerExtra.EquippedEmotes.Remove(emoteId);
            PlayerDB.Players.Update(player);

            return Ok(new { emoteId = emoteId, equipped = false });
        }

        [HttpGet("v1/emotes/equipped")]
        public IActionResult GetEquippedEmotes()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.EquippedEmotes);
        }

        [HttpPost("v1/stamp/equip/{stampId}")]
        public IActionResult EquipStamp(string stampId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedStamps == null)
                player.Player.PlayerExtra.EquippedStamps = new List<string>();

            if (!player.Player.PlayerExtra.EquippedStamps.Contains(stampId))
            {
                player.Player.PlayerExtra.EquippedStamps.Add(stampId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { stampId = stampId, equipped = true });
        }

        [HttpDelete("v1/stamp/unequip/{stampId}")]
        public IActionResult UnequipStamp(string stampId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedStamps != null)
                player.Player.PlayerExtra.EquippedStamps.Remove(stampId);

            PlayerDB.Players.Update(player);

            return Ok(new { stampId = stampId, equipped = false });
        }

        [HttpGet("v1/stamps/equipped")]
        public IActionResult GetEquippedStamps()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.EquippedStamps ?? new List<string>());
        }

        [HttpPost("v1/sticker/equip/{stickerId}")]
        public IActionResult EquipSticker(string stickerId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedStickers == null)
                player.Player.PlayerExtra.EquippedStickers = new List<string>();

            if (!player.Player.PlayerExtra.EquippedStickers.Contains(stickerId))
            {
                player.Player.PlayerExtra.EquippedStickers.Add(stickerId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { stickerId = stickerId, equipped = true });
        }

        [HttpDelete("v1/sticker/unequip/{stickerId}")]
        public IActionResult UnequipSticker(string stickerId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedStickers != null)
                player.Player.PlayerExtra.EquippedStickers.Remove(stickerId);

            PlayerDB.Players.Update(player);

            return Ok(new { stickerId = stickerId, equipped = false });
        }

        [HttpGet("v1/stickers/equipped")]
        public IActionResult GetEquippedStickers()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.EquippedStickers ?? new List<string>());
        }

        [HttpPost("v1/gadget/equip/{gadgetId}")]
        public IActionResult EquipGadget(string gadgetId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedGadgets == null)
                player.Player.PlayerExtra.EquippedGadgets = new List<string>();

            if (!player.Player.PlayerExtra.EquippedGadgets.Contains(gadgetId))
            {
                player.Player.PlayerExtra.EquippedGadgets.Add(gadgetId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { gadgetId = gadgetId, equipped = true });
        }

        [HttpDelete("v1/gadget/unequip/{gadgetId}")]
        public IActionResult UnequipGadget(string gadgetId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (player.Player.PlayerExtra.EquippedGadgets != null)
                player.Player.PlayerExtra.EquippedGadgets.Remove(gadgetId);

            PlayerDB.Players.Update(player);

            return Ok(new { gadgetId = gadgetId, equipped = false });
        }

        [HttpGet("v1/gadgets/equipped")]
        public IActionResult GetEquippedGadgets()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.EquippedGadgets ?? new List<string>());
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
                    // Backpack skins
                    new { itemId = "backpack_unicorn", name = "Unicorn Backpack", price = 500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_knight", name = "Knight Backpack", price = 300, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_ninja", name = "Ninja Backpack", price = 300, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_robot", name = "Robot Backpack", price = 250, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_rainbow", name = "Rainbow Backpack", price = 800, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_crown", name = "Crown Backpack", price = 1000, currency = "tokens", type = "backpack_skin" },
                    
                    // Paint pistols
                    new { itemId = "paint_pistol_gold", name = "Gold Paint Pistol", price = 1000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_rainbow", name = "Rainbow Paint Pistol", price = 2000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_galaxy", name = "Galaxy Paint Pistol", price = 2500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_neon", name = "Neon Paint Pistol", price = 800, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_void", name = "Void Paint Pistol", price = 3000, currency = "tokens", type = "paint_pistol" },
                    
                    // Foods
                    new { itemId = "food_pizza", name = "Pizza", price = 10, currency = "tokens", type = "food" },
                    new { itemId = "food_cake", name = "Cake", price = 25, currency = "tokens", type = "food" },
                    new { itemId = "food_burger", name = "Burger", price = 15, currency = "tokens", type = "food" },
                    new { itemId = "food_sushi", name = "Sushi", price = 40, currency = "tokens", type = "food" },
                    new { itemId = "food_icecream", name = "Ice Cream", price = 12, currency = "tokens", type = "food" },
                    
                    // Clothing
                    new { itemId = "cloth_suit", name = "Suit", price = 300, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_dress", name = "Dress", price = 250, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hat_top", name = "Top Hat", price = 200, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hoodie", name = "Hoodie", price = 150, currency = "tokens", type = "clothing" },
                    
                    // Emotes
                    new { itemId = "emote_dance", name = "Dance Emote", price = 100, currency = "tokens", type = "emote" },
                    new { itemId = "emote_celebrate", name = "Celebrate Emote", price = 200, currency = "tokens", type = "emote" },
                    new { itemId = "emote_wave", name = "Wave Emote", price = 50, currency = "tokens", type = "emote" },
                    
                    // Stamps
                    new { itemId = "stamp_like", name = "Like Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_love", name = "Love Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_wow", name = "Wow Stamp", price = 50, currency = "tokens", type = "stamp" },
                    
                    // Stickers
                    new { itemId = "sticker_star", name = "Star Sticker", price = 30, currency = "tokens", type = "sticker" },
                    new { itemId = "sticker_heart", name = "Heart Sticker", price = 30, currency = "tokens", type = "sticker" },
                    
                    // Gadgets
                    new { itemId = "gadget_balloon", name = "Balloon Gadget", price = 100, currency = "tokens", type = "gadget" },
                    new { itemId = "gadget_jetpack", name = "Jetpack Gadget", price = 5000, currency = "tokens", type = "gadget" },
                    
                    // Maker tools
                    new { itemId = "tool_material", name = "Material Tool", price = 400, currency = "tokens", type = "tool" },
                    new { itemId = "tool_light", name = "Light Tool", price = 400, currency = "tokens", type = "tool" },
                    // Backpack skins
                    new { itemId = "backpack_unicorn", name = "Unicorn Backpack", price = 500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_knight", name = "Knight Backpack", price = 300, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_ninja", name = "Ninja Backpack", price = 300, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_robot", name = "Robot Backpack", price = 250, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_rainbow", name = "Rainbow Backpack", price = 800, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_crown", name = "Crown Backpack", price = 1000, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_flame", name = "Flame Backpack", price = 600, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_ocean", name = "Ocean Backpack", price = 600, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_galaxy", name = "Galaxy Backpack", price = 1200, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_void", name = "Void Backpack", price = 1200, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_prism", name = "Prism Backpack", price = 1200, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_chrome", name = "Chrome Backpack", price = 700, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_neon", name = "Neon Backpack", price = 700, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_emerald", name = "Emerald Backpack", price = 1500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_ruby", name = "Ruby Backpack", price = 1500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_sapphire", name = "Sapphire Backpack", price = 1500, currency = "tokens", type = "backpack_skin" },
                    new { itemId = "backpack_diamond", name = "Diamond Backpack", price = 2000, currency = "tokens", type = "backpack_skin" },
                    
                    // Paint pistols
                    new { itemId = "paint_pistol_gold", name = "Gold Paint Pistol", price = 1000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_rainbow", name = "Rainbow Paint Pistol", price = 2000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_galaxy", name = "Galaxy Paint Pistol", price = 2500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_neon", name = "Neon Paint Pistol", price = 800, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_void", name = "Void Paint Pistol", price = 3000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_nebula", name = "Nebula Paint Pistol", price = 3000, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_obsidian", name = "Obsidian Paint Pistol", price = 1500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_pearl", name = "Pearl Paint Pistol", price = 400, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_ruby", name = "Ruby Paint Pistol", price = 1500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_emerald", name = "Emerald Paint Pistol", price = 1500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_sapphire", name = "Sapphire Paint Pistol", price = 1500, currency = "tokens", type = "paint_pistol" },
                    new { itemId = "paint_pistol_amethyst", name = "Amethyst Paint Pistol", price = 700, currency = "tokens", type = "paint_pistol" },
                    
                    // Foods
                    new { itemId = "food_pizza", name = "Pizza", price = 10, currency = "tokens", type = "food" },
                    new { itemId = "food_cake", name = "Cake", price = 25, currency = "tokens", type = "food" },
                    new { itemId = "food_burger", name = "Burger", price = 15, currency = "tokens", type = "food" },
                    new { itemId = "food_sushi", name = "Sushi", price = 40, currency = "tokens", type = "food" },
                    new { itemId = "food_icecream", name = "Ice Cream", price = 12, currency = "tokens", type = "food" },
                    new { itemId = "food_tacos", name = "Tacos", price = 18, currency = "tokens", type = "food" },
                    new { itemId = "food_burrito", name = "Burrito", price = 20, currency = "tokens", type = "food" },
                    new { itemId = "food_quesadilla", name = "Quesadilla", price = 22, currency = "tokens", type = "food" },
                    new { itemId = "food_nachos", name = "Nachos", price = 20, currency = "tokens", type = "food" },
                    new { itemId = "food_enchilada", name = "Enchilada", price = 35, currency = "tokens", type = "food" },
                    new { itemId = "food_tamale", name = "Tamale", price = 35, currency = "tokens", type = "food" },
                    new { itemId = "food_ceviche", name = "Ceviche", price = 55, currency = "tokens", type = "food" },
                    new { itemId = "food_quindim", name = "Quindim", price = 60, currency = "tokens", type = "food" },
                    
                    // Clothing
                    new { itemId = "cloth_suit", name = "Suit", price = 300, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_dress", name = "Dress", price = 250, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hat_top", name = "Top Hat", price = 200, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hoodie", name = "Hoodie", price = 150, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_shirt_flannel", name = "Flannel Shirt", price = 120, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_shirt_sweater", name = "Sweater", price = 180, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_shirt_blazer", name = "Blazer", price = 400, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_pants_dress", name = "Dress Pants", price = 200, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_pants_cargo", name = "Cargo Pants", price = 180, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hat_fedora", name = "Fedora", price = 160, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hat_wizard_hat", name = "Wizard Hat", price = 450, currency = "tokens", type = "clothing" },
                    new { itemId = "cloth_hat_bunny_ears", name = "Bunny Ears", price = 100, currency = "tokens", type = "clothing" },
                    
                    // Emotes
                    new { itemId = "emote_dance", name = "Dance Emote", price = 100, currency = "tokens", type = "emote" },
                    new { itemId = "emote_celebrate", name = "Celebrate Emote", price = 200, currency = "tokens", type = "emote" },
                    new { itemId = "emote_wave", name = "Wave Emote", price = 50, currency = "tokens", type = "emote" },
                    new { itemId = "emote_clap", name = "Clap Emote", price = 30, currency = "tokens", type = "emote" },
                    new { itemId = "emote_facepalm", name = "Facepalm Emote", price = 80, currency = "tokens", type = "emote" },
                    new { itemId = "emote_bow", name = "Bow Emote", price = 100, currency = "tokens", type = "emote" },
                    new { itemId = "emote_laugh", name = "Laugh Emote", price = 60, currency = "tokens", type = "emote" },
                    
                    // Stamps
                    new { itemId = "stamp_like", name = "Like Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_love", name = "Love Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_wow", name = "Wow Stamp", price = 50, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_haha", name = "Haha Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_sad", name = "Sad Stamp", price = 25, currency = "tokens", type = "stamp" },
                    new { itemId = "stamp_angry", name = "Angry Stamp", price = 50, currency = "tokens", type = "stamp" },
                    
                    // Stickers
                    new { itemId = "sticker_star", name = "Star Sticker", price = 30, currency = "tokens", type = "sticker" },
                    new { itemId = "sticker_heart", name = "Heart Sticker", price = 30, currency = "tokens", type = "sticker" },
                    new { itemId = "sticker_crown", name = "Crown Sticker", price = 60, currency = "tokens", type = "sticker" },
                    new { itemId = "sticker_skull", name = "Skull Sticker", price = 60, currency = "tokens", type = "sticker" },
                    
                    // Gadgets
                    new { itemId = "gadget_balloon", name = "Balloon Gadget", price = 100, currency = "tokens", type = "gadget" },
                    new { itemId = "gadget_jetpack", name = "Jetpack Gadget", price = 5000, currency = "tokens", type = "gadget" },
                    new { itemId = "gadget_glider", name = "Glider Gadget", price = 2500, currency = "tokens", type = "gadget" },
                    new { itemId = "gadget_hookshot", name = "Hookshot Gadget", price = 2500, currency = "tokens", type = "gadget" },
                    
                    // Maker tools
                    new { itemId = "tool_material", name = "Material Tool", price = 400, currency = "tokens", type = "tool" },
                    new { itemId = "tool_light", name = "Light Tool", price = 400, currency = "tokens", type = "tool" },
                    new { itemId = "tool_color_picker", name = "Color Picker Tool", price = 400, currency = "tokens", type = "tool" },
                    new { itemId = "tool_group", name = "Group Tool", price = 600, currency = "tokens", type = "tool" },
                },
                TotalResults = 80
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
