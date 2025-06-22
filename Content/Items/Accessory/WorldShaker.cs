﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

using Luxcinder.Content.Items.QuenchedSeries;

namespace Luxcinder.Content.Items.Accessory
{
    /// <summary>
    /// 旷世者 - 专家级饰品，提供强大的战斗增益
    /// </summary>
    public class WorldShaker : ModItem
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
            Item.rare = ItemRarityID.Expert;
            Item.defense = 5;
        
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
        /// - 免疫燃烧和击退
        /// - 提升10%全伤害
        /// - 提升50%近战暴击率和攻击速度
        /// - 生命值≥200时获得额外35防御和5%伤害
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual) 
        {
            // 基础效果
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.noKnockback = true;
        
            // 战斗增强
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Melee) += 0.50f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.50f;

            // 高生命值奖励
            if (player.statLifeMax2 >= 200)
            {
                player.statDefense += 35;
                player.GetDamage(DamageClass.Generic) += 0.05f;
            }
        }

        public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient<QuenchedIngot>(10)
                .Register();
        }
    }
}