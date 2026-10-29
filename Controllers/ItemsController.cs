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
                total = BackpackSkins.Count + PaintPistols.Count + Foods.Count + MakerTools.Count + Clothing.Count
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

        [HttpGet("v1/{itemId}")]
        public IActionResult GetItem(string itemId)
        {
            foreach (var list in new[] { BackpackSkins, PaintPistols, Foods, MakerTools, Clothing })
            {
                var item = list.FirstOrDefault(i => 
                {
                    var dict = i as Dictionary<string, object>;
                    if (dict != null && dict.ContainsKey("id")) return dict["id"].ToString() == itemId;
                    return false;
                }) ?? list.FirstOrDefault(i => i.GetType().GetProperty("id")?.GetValue(i)?.ToString() == itemId);
                if (item != null) return Ok(item);
            }
            return NotFound();
        }
    }
}
