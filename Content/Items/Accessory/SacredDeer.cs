﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Accessory
{
    /// <summary>
    /// 圣鹿庇护者 - 提供多种防御效果和昼夜不同的增益
    /// </summary>
    public class SacredDeer : ModItem
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
            Item.rare = ItemRarityID.Pink;
            Item.defense = 6;
        
            // 使用属性
            Item.useStyle = ItemUseStyleID.None;//饰品无法挥动
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.placeStyle = 0;
        
            // 饰品属性
            Item.accessory = true;
        
            // 价值(35金币)
            Item.value = Item.sellPrice(gold: 35);
        }

        /// <summary>
        /// 装备效果：
        /// - 免疫多种debuff
        /// - 根据昼夜提供不同效果
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual) 
        {
            // 基础免疫效果
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Blackout] = true;
        
            // 常驻效果
            player.AddBuff(BuffID.Shine, 1);
            player.GetDamage(DamageClass.Generic) -= 0.05f;
            player.GetDamage(DamageClass.Magic) -= 0.50f;

            // 困难模式下的昼夜效果
            if (Main.hardMode)
            {
                if (Main.dayTime)
                {
                    player.lifeRegen += 1;
                    player.statLifeMax2 += 50;
                }
                else
                {
                    player.moveSpeed += 0.10f;
                    player.AddBuff(BuffID.Swiftness, 1);
                }
            }
        }

        public override void AddRecipes() 
        {
            CreateRecipe()
            .AddIngredient(ItemID.SoulofFlight, 15)
            .AddIngredient(ItemID.SoulofFright, 15)
            .AddIngredient(ItemID.CrystalShard, 35)
            .AddIngredient(ItemID.SoulofLight, 15)
            .AddIngredient(ItemID.SoulofNight, 15)
            .AddTile(TileID.MythrilAnvil) // 秘银砧
                .Register();
        }
    }
}
