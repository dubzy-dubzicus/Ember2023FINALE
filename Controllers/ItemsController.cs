using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/items")]
    public class ItemsController : ControllerBase
    {
        // All backpack skins from Rec Room
        private static readonly List<object> BackpackSkins = new()
        {
            new { id = "backpack_default", name = "Default Backpack", type = "backpack_skin", rarity = "common", imageName = "backpack_default.png" },
            new { id = "backpack_plumber", name = "Plumber Backpack", type = "backpack_skin", rarity = "common", imageName = "backpack_plumber.png" },
            new { id = "backpack_cowboy", name = "Cowboy Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_cowboy.png" },
            new { id = "backpack_knight", name = "Knight Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_knight.png" },
            new { id = "backpack_ninja", name = "Ninja Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_ninja.png" },
            new { id = "backpack_robot", name = "Robot Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_robot.png" },
            new { id = "backpack_unicorn", name = "Unicorn Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_unicorn.png" },
            new { id = "backpack_pumpkin", name = "Pumpkin Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_pumpkin.png" },
            new { id = "backpack_santa", name = "Santa Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_santa.png" },
            new { id = "backpack_heart", name = "Heart Backpack", type = "backpack_skin", rarity = "common", imageName = "backpack_heart.png" },
            new { id = "backpack_star", name = "Star Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_star.png" },
            new { id = "backpack_moon", name = "Moon Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_moon.png" },
            new { id = "backpack_cloud", name = "Cloud Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_cloud.png" },
            new { id = "backpack_rainbow", name = "Rainbow Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_rainbow.png" },
            new { id = "backpack_skull", name = "Skull Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_skull.png" },
            new { id = "backpack_crown", name = "Crown Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_crown.png" },
            new { id = "backpack_gear", name = "Gear Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_gear.png" },
            new { id = "backpack_leaf", name = "Leaf Backpack", type = "backpack_skin", rarity = "common", imageName = "backpack_leaf.png" },
            new { id = "backpack_flame", name = "Flame Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_flame.png" },
        };

        // All paint pistols and colors
        private static readonly List<object> PaintPistols = new()
        {
            new { id = "paint_pistol_red", name = "Red Paint Pistol", type = "paint_pistol", rarity = "common", color = "red", imageName = "paint_pistol_red.png" },
            new { id = "paint_pistol_blue", name = "Blue Paint Pistol", type = "paint_pistol", rarity = "common", color = "blue", imageName = "paint_pistol_blue.png" },
            new { id = "paint_pistol_green", name = "Green Paint Pistol", type = "paint_pistol", rarity = "common", color = "green", imageName = "paint_pistol_green.png" },
            new { id = "paint_pistol_yellow", name = "Yellow Paint Pistol", type = "paint_pistol", rarity = "common", color = "yellow", imageName = "paint_pistol_yellow.png" },
            new { id = "paint_pistol_purple", name = "Purple Paint Pistol", type = "paint_pistol", rarity = "common", color = "purple", imageName = "paint_pistol_purple.png" },
            new { id = "paint_pistol_black", name = "Black Paint Pistol", type = "paint_pistol", rarity = "common", color = "black", imageName = "paint_pistol_black.png" },
            new { id = "paint_pistol_white", name = "White Paint Pistol", type = "paint_pistol", rarity = "common", color = "white", imageName = "paint_pistol_white.png" },
            new { id = "paint_pistol_gold", name = "Gold Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "gold", imageName = "paint_pistol_gold.png" },
            new { id = "paint_pistol_rainbow", name = "Rainbow Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "rainbow", imageName = "paint_pistol_rainbow.png" },
            new { id = "paint_pistol_silver", name = "Silver Paint Pistol", type = "paint_pistol", rarity = "rare", color = "silver", imageName = "paint_pistol_silver.png" },
            new { id = "paint_pistol_bronze", name = "Bronze Paint Pistol", type = "paint_pistol", rarity = "rare", color = "bronze", imageName = "paint_pistol_bronze.png" },
            new { id = "paint_pistol_copper", name = "Copper Paint Pistol", type = "paint_pistol", rarity = "rare", color = "copper", imageName = "paint_pistol_copper.png" },
            new { id = "paint_pistol_neon", name = "Neon Paint Pistol", type = "paint_pistol", rarity = "epic", color = "neon", imageName = "paint_pistol_neon.png" },
            new { id = "paint_pistol_galaxy", name = "Galaxy Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "galaxy", imageName = "paint_pistol_galaxy.png" },
        };

        // All food/consumable items
        private static readonly List<object> Foods = new()
        {
            new { id = "food_pizza", name = "Pizza", type = "food", rarity = "common", imageName = "food_pizza.png" },
            new { id = "food_burger", name = "Burger", type = "food", rarity = "common", imageName = "food_burger.png" },
            new { id = "food_taco", name = "Taco", type = "food", rarity = "common", imageName = "food_taco.png" },
            new { id = "food_sushi", name = "Sushi", type = "food", rarity = "rare", imageName = "food_sushi.png" },
            new { id = "food_cake", name = "Cake", type = "food", rarity = "rare", imageName = "food_cake.png" },
            new { id = "food_icecream", name = "Ice Cream", type = "food", rarity = "common", imageName = "food_icecream.png" },
            new { id = "food_apple", name = "Apple", type = "food", rarity = "common", imageName = "food_apple.png" },
            new { id = "food_donut", name = "Donut", type = "food", rarity = "common", imageName = "food_donut.png" },
            new { id = "food_hotdog", name = "Hot Dog", type = "food", rarity = "common", imageName = "food_hotdog.png" },
            new { id = "food_taco_supreme", name = "Supreme Taco", type = "food", rarity = "rare", imageName = "food_taco_supreme.png" },
            new { id = "food_sushi_platter", name = "Sushi Platter", type = "food", rarity = "epic", imageName = "food_sushi_platter.png" },
            new { id = "food_cake_chocolate", name = "Chocolate Cake", type = "food", rarity = "rare", imageName = "food_cake_chocolate.png" },
            new { id = "food_icecream_cone", name = "Ice Cream Cone", type = "food", rarity = "common", imageName = "food_icecream_cone.png" },
            new { id = "food_apple_red", name = "Red Apple", type = "food", rarity = "common", imageName = "food_apple_red.png" },
            new { id = "food_donut_chocolate", name = "Chocolate Donut", type = "food", rarity = "common", imageName = "food_donut_chocolate.png" },
            new { id = "food_burger_cheese", name = "Cheeseburger", type = "food", rarity = "common", imageName = "food_burger_cheese.png" },
            new { id = "food_pizza_pepperoni", name = "Pepperoni Pizza", type = "food", rarity = "rare", imageName = "food_pizza_pepperoni.png" },
            new { id = "food_salad", name = "Salad", type = "food", rarity = "common", imageName = "food_salad.png" },
        };

        // All maker pen tools
        private static readonly List<object> MakerTools = new()
        {
            new { id = "tool_maker_pen", name = "Maker Pen", type = "tool", rarity = "common", imageName = "tool_maker_pen.png" },
            new { id = "tool_paint_pistol", name = "Paint Pistol Tool", type = "tool", rarity = "common", imageName = "tool_paint_pistol.png" },
            new { id = "tool_shapes", name = "Shapes Tool", type = "tool", rarity = "common", imageName = "tool_shapes.png" },
            new { id = "tool_eraser", name = "Eraser Tool", type = "tool", rarity = "common", imageName = "tool_eraser.png" },
            new { id = "tool_select", name = "Select Tool", type = "tool", rarity = "common", imageName = "tool_select.png" },
            new { id = "tool_color", name = "Color Tool", type = "tool", rarity = "common", imageName = "tool_color.png" },
            new { id = "tool_material", name = "Material Tool", type = "tool", rarity = "rare", imageName = "tool_material.png" },
            new { id = "tool_light", name = "Light Tool", type = "tool", rarity = "rare", imageName = "tool_light.png" },
            new { id = "tool_position", name = "Position Tool", type = "tool", rarity = "rare", imageName = "tool_position.png" },
            new { id = "tool_rotation", name = "Rotation Tool", type = "tool", rarity = "rare", imageName = "tool_rotation.png" },
            new { id = "tool_scale", name = "Scale Tool", type = "tool", rarity = "rare", imageName = "tool_scale.png" },
            new { id = "tool_color_picker", name = "Color Picker Tool", type = "tool", rarity = "rare", imageName = "tool_color_picker.png" },
            new { id = "tool_group", name = "Group Tool", type = "tool", rarity = "epic", imageName = "tool_group.png" },
            new { id = "tool_ungroup", name = "Ungroup Tool", type = "tool", rarity = "epic", imageName = "tool_ungroup.png" },
            new { id = "tool_circuit_board", name = "Circuit Board Tool", type = "tool", rarity = "epic", imageName = "tool_circuit_board.png" },
            new { id = "tool_microchip", name = "Microchip Tool", type = "tool", rarity = "epic", imageName = "tool_microchip.png" },
            new { id = "tool_global_seed", name = "Global Seed Tool", type = "tool", rarity = "legendary", imageName = "tool_global_seed.png" },
        };

        // All clothing items
        private static readonly List<object> Clothing = new()
        {
            new { id = "cloth_shirt_basic", name = "Basic Shirt", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_basic.png" },
            new { id = "cloth_pants_basic", name = "Basic Pants", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_basic.png" },
            new { id = "cloth_hat_cap", name = "Baseball Cap", type = "clothing", slot = "hat", rarity = "common", imageName = "cloth_hat_cap.png" },
            new { id = "cloth_hat_cowboy", name = "Cowboy Hat", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_cowboy.png" },
            new { id = "cloth_hat_top", name = "Top Hat", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_top.png" },
            new { id = "cloth_dress", name = "Dress", type = "clothing", slot = "full_body", rarity = "rare", imageName = "cloth_dress.png" },
            new { id = "cloth_suit", name = "Suit", type = "clothing", slot = "full_body", rarity = "epic", imageName = "cloth_suit.png" },
            new { id = "cloth_hoodie", name = "Hoodie", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_hoodie.png" },
            new { id = "cloth_shirt_polo", name = "Polo Shirt", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_polo.png" },
            new { id = "cloth_shirt_tank", name = "Tank Top", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_tank.png" },
            new { id = "cloth_pants_jeans", name = "Jeans", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_jeans.png" },
            new { id = "cloth_pants_shorts", name = "Shorts", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_shorts.png" },
            new { id = "cloth_hat_beanie", name = "Beanie", type = "clothing", slot = "hat", rarity = "common", imageName = "cloth_hat_beanie.png" },
            new { id = "cloth_hat_beret", name = "Beret", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_beret.png" },
            new { id = "cloth_hat_wizard", name = "Wizard Hat", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_wizard.png" },
            new { id = "cloth_dress_formal", name = "Formal Dress", type = "clothing", slot = "full_body", rarity = "epic", imageName = "cloth_dress_formal.png" },
            new { id = "cloth_suit_tuxedo", name = "Tuxedo", type = "clothing", slot = "full_body", rarity = "legendary", imageName = "cloth_suit_tuxedo.png" },
            new { id = "cloth_jacket_leather", name = "Leather Jacket", type = "clothing", slot = "shirt", rarity = "rare", imageName = "cloth_jacket_leather.png" },
            new { id = "cloth_vest", name = "Vest", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_vest.png" },
        };

        // All emotes
        private static readonly List<object> Emotes = new()
        {
            new { id = "emote_wave", name = "Wave", type = "emote", rarity = "common", imageName = "emote_wave.png" },
            new { id = "emote_dance", name = "Dance", type = "emote", rarity = "common", imageName = "emote_dance.png" },
            new { id = "emote_clap", name = "Clap", type = "emote", rarity = "common", imageName = "emote_clap.png" },
            new { id = "emote_thumbs_up", name = "Thumbs Up", type = "emote", rarity = "common", imageName = "emote_thumbs_up.png" },
            new { id = "emote_facepalm", name = "Facepalm", type = "emote", rarity = "rare", imageName = "emote_facepalm.png" },
            new { id = "emote_bow", name = "Bow", type = "emote", rarity = "rare", imageName = "emote_bow.png" },
            new { id = "emote_celebrate", name = "Celebrate", type = "emote", rarity = "epic", imageName = "emote_celebrate.png" },
            new { id = "emote_laugh", name = "Laugh", type = "emote", rarity = "common", imageName = "emote_laugh.png" },
        };

        // All stamps
        private static readonly List<object> Stamps = new()
        {
            new { id = "stamp_like", name = "Like Stamp", type = "stamp", rarity = "common", imageName = "stamp_like.png" },
            new { id = "stamp_love", name = "Love Stamp", type = "stamp", rarity = "common", imageName = "stamp_love.png" },
            new { id = "stamp_haha", name = "Haha Stamp", type = "stamp", rarity = "common", imageName = "stamp_haha.png" },
            new { id = "stamp_wow", name = "Wow Stamp", type = "stamp", rarity = "rare", imageName = "stamp_wow.png" },
            new { id = "stamp_sad", name = "Sad Stamp", type = "stamp", rarity = "common", imageName = "stamp_sad.png" },
            new { id = "stamp_angry", name = "Angry Stamp", type = "stamp", rarity = "rare", imageName = "stamp_angry.png" },
        };

        // All stickers
        private static readonly List<object> Stickers = new()
        {
            new { id = "sticker_star", name = "Star Sticker", type = "sticker", rarity = "common", imageName = "sticker_star.png" },
            new { id = "sticker_heart", name = "Heart Sticker", type = "sticker", rarity = "common", imageName = "sticker_heart.png" },
            new { id = "sticker_crown", name = "Crown Sticker", type = "sticker", rarity = "rare", imageName = "sticker_crown.png" },
            new { id = "sticker_skull", name = "Skull Sticker", type = "sticker", rarity = "rare", imageName = "sticker_skull.png" },
        };

        // All gadgets
        private static readonly List<object> Gadgets = new()
        {
            new { id = "gadget_jetpack", name = "Jetpack", type = "gadget", rarity = "legendary", imageName = "gadget_jetpack.png" },
            new { id = "gadget_glider", name = "Glider", type = "gadget", rarity = "epic", imageName = "gadget_glider.png" },
            new { id = "gadget_hookshot", name = "Hookshot", type = "gadget", rarity = "epic", imageName = "gadget_hookshot.png" },
            new { id = "gadget_balloon", name = "Balloon", type = "gadget", rarity = "common", imageName = "gadget_balloon.png" },
        };

        [HttpGet("v1/all")]
        public IActionResult GetAllItems()
        {
            return Ok(new
            {
                backpackSkins = BackpackSkins,
                paintPistols = PaintPistols,
                foods = Foods,
                makerTools = MakerTools,
                clothing = Clothing,
                emotes = Emotes,
                stamps = Stamps,
                stickers = Stickers,
                gadgets = Gadgets,
                total = BackpackSkins.Count + PaintPistols.Count + Foods.Count + MakerTools.Count + Clothing.Count + Emotes.Count + Stamps.Count + Stickers.Count + Gadgets.Count
            });
        }

        [HttpGet("v1/backpackSkins")]
        public IActionResult GetBackpackSkins() => Ok(BackpackSkins);

        [HttpGet("v1/paintPistols")]
        public IActionResult GetPaintPistols() => Ok(PaintPistols);

        [HttpGet("v1/foods")]
        public IActionResult GetFoods() => Ok(Foods);

        [HttpGet("v1/makerTools")]
        public IActionResult GetMakerTools() => Ok(MakerTools);

        [HttpGet("v1/clothing")]
        public IActionResult GetClothing() => Ok(Clothing);

        [HttpGet("v1/emotes")]
        public IActionResult GetEmotes() => Ok(Emotes);

        [HttpGet("v1/stamps")]
        public IActionResult GetStamps() => Ok(Stamps);

        [HttpGet("v1/stickers")]
        public IActionResult GetStickers() => Ok(Stickers);

        [HttpGet("v1/gadgets")]
        public IActionResult GetGadgets() => Ok(Gadgets);

        [HttpGet("v1/{itemId}")]
        public IActionResult GetItem(string itemId)
        {
            foreach (var list in new[] { BackpackSkins, PaintPistols, Foods, MakerTools, Clothing, Emotes, Stamps, Stickers, Gadgets })
            {
                var item = list.FirstOrDefault(i => i.GetType().GetProperty("id")?.GetValue(i)?.ToString() == itemId);
                if (item != null) return Ok(item);
            }
            return NotFound();
        }
    }
}
