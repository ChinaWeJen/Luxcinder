﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Accessory
{
    /// <summary>
    /// 守护者之盾 - 提供生命值加成和燃烧免疫
    /// </summary>
    public class GuardianShield : ModItem
    {
        public override void SetStaticDefaults() 
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // 影响物品栏排序(59=铂金锭，数值越高越靠后)
        }

        public override void SetDefaults() 
        {
            // 基础属性
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
        
            // 使用属性
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.placeStyle = 0;
        
            // 饰品属性
            Item.accessory = true;
        
            // 价值(35金币)
            Item.value = Item.sellPrice(gold: 35);
        }

        /// <summary>
        /// 装备效果：
        /// - 增加40点最大生命值
        /// - 提升生命恢复速度
        /// - 免疫燃烧效果
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual) 
        {
            player.statLifeMax2 += 40;
            player.lifeRegen += 1;
            player.buffImmune[BuffID.Burning] = true;
        }

        public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient(ItemID.ObsidianShield, 1)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile(TileID.TinkerersWorkbench) // 工匠作坊
                .Register();
        }
    }
}