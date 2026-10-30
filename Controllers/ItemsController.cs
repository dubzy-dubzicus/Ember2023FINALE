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
            new { id = "backpack_ocean", name = "Ocean Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_ocean.png" },
            new { id = "backpack_galaxy", name = "Galaxy Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_galaxy.png" },
            new { id = "backpack_void", name = "Void Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_void.png" },
            new { id = "backpack_prism", name = "Prism Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_prism.png" },
            new { id = "backpack_chrome", name = "Chrome Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_chrome.png" },
            new { id = "backpack_neon", name = "Neon Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_neon.png" },
            new { id = "backpack_steel", name = "Steel Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_steel.png" },
            new { id = "backpack_wood", name = "Wood Backpack", type = "backpack_skin", rarity = "common", imageName = "backpack_wood.png" },
            new { id = "backpack_stone", name = "Stone Backpack", type = "backpack_skin", rarity = "rare", imageName = "backpack_stone.png" },
            new { id = "backpack_crystal", name = "Crystal Backpack", type = "backpack_skin", rarity = "epic", imageName = "backpack_crystal.png" },
            new { id = "backpack_emerald", name = "Emerald Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_emerald.png" },
            new { id = "backpack_ruby", name = "Ruby Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_ruby.png" },
            new { id = "backpack_sapphire", name = "Sapphire Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_sapphire.png" },
            new { id = "backpack_diamond", name = "Diamond Backpack", type = "backpack_skin", rarity = "legendary", imageName = "backpack_diamond.png" },
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
            new { id = "paint_pistol_chrome", name = "Chrome Paint Pistol", type = "paint_pistol", rarity = "epic", color = "chrome", imageName = "paint_pistol_chrome.png" },
            new { id = "paint_pistol_matte", name = "Matte Paint Pistol", type = "paint_pistol", rarity = "rare", color = "matte", imageName = "paint_pistol_matte.png" },
            new { id = "paint_pistol_glow", name = "Glow Paint Pistol", type = "paint_pistol", rarity = "epic", color = "glow", imageName = "paint_pistol_glow.png" },
            new { id = "paint_pistol_void", name = "Void Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "void", imageName = "paint_pistol_void.png" },
            new { id = "paint_pistol_prism", name = "Prism Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "prism", imageName = "paint_pistol_prism.png" },
            new { id = "paint_pistol_chrome", name = "Chrome Paint Pistol", type = "paint_pistol", rarity = "epic", color = "chrome", imageName = "paint_pistol_chrome.png" },
            new { id = "paint_pistol_matte", name = "Matte Paint Pistol", type = "paint_pistol", rarity = "rare", color = "matte", imageName = "paint_pistol_matte.png" },
            new { id = "paint_pistol_glow", name = "Glow Paint Pistol", type = "paint_pistol", rarity = "epic", color = "glow", imageName = "paint_pistol_glow.png" },
            new { id = "paint_pistol_void", name = "Void Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "void", imageName = "paint_pistol_void.png" },
            new { id = "paint_pistol_nebula", name = "Nebula Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "nebula", imageName = "paint_pistol_nebula.png" },
            new { id = "paint_pistol_obsidian", name = "Obsidian Paint Pistol", type = "paint_pistol", rarity = "epic", color = "obsidian", imageName = "paint_pistol_obsidian.png" },
            new { id = "paint_pistol_pearl", name = "Pearl Paint Pistol", type = "paint_pistol", rarity = "rare", color = "pearl", imageName = "paint_pistol_pearl.png" },
            new { id = "paint_pistol_ruby", name = "Ruby Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "ruby", imageName = "paint_pistol_ruby.png" },
            new { id = "paint_pistol_emerald", name = "Emerald Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "emerald", imageName = "paint_pistol_emerald.png" },
            new { id = "paint_pistol_sapphire", name = "Sapphire Paint Pistol", type = "paint_pistol", rarity = "legendary", color = "sapphire", imageName = "paint_pistol_sapphire.png" },
            new { id = "paint_pistol_amethyst", name = "Amethyst Paint Pistol", type = "paint_pistol", rarity = "epic", color = "amethyst", imageName = "paint_pistol_amethyst.png" },
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
            new { id = "food_sandwich", name = "Sandwich", type = "food", rarity = "common", imageName = "food_sandwich.png" },
            new { id = "food_soup", name = "Soup", type = "food", rarity = "common", imageName = "food_soup.png" },
            new { id = "food_steak", name = "Steak", type = "food", rarity = "rare", imageName = "food_steak.png" },
            new { id = "food_sushi_roll", name = "Sushi Roll", type = "food", rarity = "rare", imageName = "food_sushi_roll.png" },
            new { id = "food_cupcake", name = "Cupcake", type = "food", rarity = "common", imageName = "food_cupcake.png" },
            new { id = "food_cookie", name = "Cookie", type = "food", rarity = "common", imageName = "food_cookie.png" },
            new { id = "food_pie", name = "Pie", type = "food", rarity = "rare", imageName = "food_pie.png" },
            new { id = "food_pancakes", name = "Pancakes", type = "food", rarity = "common", imageName = "food_pancakes.png" },
            new { id = "food_waffles", name = "Waffles", type = "food", rarity = "common", imageName = "food_waffles.png" },
            new { id = "food_tacos", name = "Tacos", type = "food", rarity = "common", imageName = "food_tacos.png" },
            new { id = "food_burrito", name = "Burrito", type = "food", rarity = "common", imageName = "food_burrito.png" },
            new { id = "food_quesadilla", name = "Quesadilla", type = "food", rarity = "common", imageName = "food_quesadilla.png" },
            new { id = "food_nachos", name = "Nachos", type = "food", rarity = "common", imageName = "food_nachos.png" },
            new { id = "food_guacamole", name = "Guacamole", type = "food", rarity = "common", imageName = "food_guacamole.png" },
            new { id = "food_salsa", name = "Salsa", type = "food", rarity = "common", imageName = "food_salsa.png" },
            new { id = "food_enchilada", name = "Enchilada", type = "food", rarity = "rare", imageName = "food_enchilada.png" },
            new { id = "food_tamale", name = "Tamale", type = "food", rarity = "rare", imageName = "food_tamale.png" },
            new { id = "food_empanada", name = "Empanada", type = "food", rarity = "rare", imageName = "food_empanada.png" },
            new { id = "food_arepa", name = "Arepa", type = "food", rarity = "rare", imageName = "food_arepa.png" },
            new { id = "food_pupusa", name = "Pupusa", type = "food", rarity = "rare", imageName = "food_pupusa.png" },
            new { id = "food_ceviche", name = "Ceviche", type = "food", rarity = "epic", imageName = "food_ceviche.png" },
            new { id = "food_pao_de_queijo", name = "Pao de Queijo", type = "food", rarity = "rare", imageName = "food_pao_de_queijo.png" },
            new { id = "food_coxinha", name = "Coxinha", type = "food", rarity = "rare", imageName = "food_coxinha.png" },
            new { id = "food_brigadeiro", name = "Brigadeiro", type = "food", rarity = "rare", imageName = "food_brigadeiro.png" },
            new { id = "food_beijinho", name = "Beijinho", type = "food", rarity = "rare", imageName = "food_beijinho.png" },
            new { id = "food_quindim", name = "Quindim", type = "food", rarity = "epic", imageName = "food_quindim.png" },
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
            new { id = "cloth_shirt_flannel", name = "Flannel Shirt", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_flannel.png" },
            new { id = "cloth_shirt_henley", name = "Henley Shirt", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_henley.png" },
            new { id = "cloth_shirt_polo_long", name = "Long Sleeve Polo", type = "clothing", slot = "shirt", rarity = "common", imageName = "cloth_shirt_polo_long.png" },
            new { id = "cloth_shirt_turtleneck", name = "Turtleneck", type = "clothing", slot = "shirt", rarity = "rare", imageName = "cloth_shirt_turtleneck.png" },
            new { id = "cloth_shirt_sweater", name = "Sweater", type = "clothing", slot = "shirt", rarity = "rare", imageName = "cloth_shirt_sweater.png" },
            new { id = "cloth_shirt_cardigan", name = "Cardigan", type = "clothing", slot = "shirt", rarity = "rare", imageName = "cloth_shirt_cardigan.png" },
            new { id = "cloth_shirt_blazer", name = "Blazer", type = "clothing", slot = "shirt", rarity = "epic", imageName = "cloth_shirt_blazer.png" },
            new { id = "cloth_shirt_tuxedo_jacket", name = "Tuxedo Jacket", type = "clothing", slot = "shirt", rarity = "legendary", imageName = "cloth_shirt_tuxedo_jacket.png" },
            new { id = "cloth_pants_dress", name = "Dress Pants", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_dress.png" },
            new { id = "cloth_pants_cargo", name = "Cargo Pants", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_cargo.png" },
            new { id = "cloth_pants_chino", name = "Chino Pants", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_chino.png" },
            new { id = "cloth_pants_sweatpants", name = "Sweatpants", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_sweatpants.png" },
            new { id = "cloth_pants_leggings", name = "Leggings", type = "clothing", slot = "pants", rarity = "common", imageName = "cloth_pants_leggings.png" },
            new { id = "cloth_pants_skirt", name = "Skirt", type = "clothing", slot = "pants", rarity = "rare", imageName = "cloth_pants_skirt.png" },
            new { id = "cloth_pants_kilt", name = "Kilt", type = "clothing", slot = "pants", rarity = "rare", imageName = "cloth_pants_kilt.png" },
            new { id = "cloth_hat_beanie_winter", name = "Winter Beanie", type = "clothing", slot = "hat", rarity = "common", imageName = "cloth_hat_beanie_winter.png" },
            new { id = "cloth_hat_fedora", name = "Fedora", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_fedora.png" },
            new { id = "cloth_hat_trilby", name = "Trilby", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_trilby.png" },
            new { id = "cloth_hat_bowler", name = "Bowler Hat", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_bowler.png" },
            new { id = "cloth_hat_top_hat", name = "Top Hat", type = "clothing", slot = "hat", rarity = "legendary", imageName = "cloth_hat_top_hat.png" },
            new { id = "cloth_hat_wizard_hat", name = "Wizard Hat", type = "clothing", slot = "hat", rarity = "legendary", imageName = "cloth_hat_wizard_hat.png" },
            new { id = "cloth_hat_witch_hat", name = "Witch Hat", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_witch_hat.png" },
            new { id = "cloth_hat_santa_hat", name = "Santa Hat", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_santa_hat.png" },
            new { id = "cloth_hat_bunny_ears", name = "Bunny Ears", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_bunny_ears.png" },
            new { id = "cloth_hat_devil_horns", name = "Devil Horns", type = "clothing", slot = "hat", rarity = "rare", imageName = "cloth_hat_devil_horns.png" },
            new { id = "cloth_hat_angel_halo", name = "Angel Halo", type = "clothing", slot = "hat", rarity = "epic", imageName = "cloth_hat_angel_halo.png" },
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
            Response.Headers.Append("Cache-Control", "public, max-age=600");
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
        public IActionResult GetBackpackSkins()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(BackpackSkins);
        }

        [HttpGet("v1/paintPistols")]
        public IActionResult GetPaintPistols()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(PaintPistols);
        }

        [HttpGet("v1/foods")]
        public IActionResult GetFoods()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Foods);
        }

        [HttpGet("v1/makerTools")]
        public IActionResult GetMakerTools()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(MakerTools);
        }

        [HttpGet("v1/clothing")]
        public IActionResult GetClothing()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Clothing);
        }

        [HttpGet("v1/emotes")]
        public IActionResult GetEmotes()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Emotes);
        }

        [HttpGet("v1/stamps")]
        public IActionResult GetStamps()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Stamps);
        }

        [HttpGet("v1/stickers")]
        public IActionResult GetStickers()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Stickers);
        }

        [HttpGet("v1/gadgets")]
        public IActionResult GetGadgets()
        {
            Response.Headers.Append("Cache-Control", "public, max-age=600");
            return Ok(Gadgets);
        }

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
