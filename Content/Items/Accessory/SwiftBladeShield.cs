﻿using Terraria;
 using Terraria.GameContent.Creative;
 using Terraria.ID;
 using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Accessory
 {
     /// <summary>
     /// 疾刃之盾 - 提供移动速度和近战增强效果
     /// </summary>
     public class SwiftBladeShield : ModItem
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
             Item.rare = ItemRarityID.Blue;
             Item.defense = 1;
            
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
         /// - 免疫击退
         /// - 增加10%近战攻击速度
         /// - 增加15%移动速度
         /// - 增加8%近战暴击率
         /// </summary>
         public override void UpdateAccessory(Player player, bool hideVisual) 
         {
             player.noKnockback = true;
             player.GetAttackSpeed(DamageClass.Melee) += 0.10f;
             player.moveSpeed += 0.15f;
             player.GetCritChance(DamageClass.Melee) += 0.08f;
         }

        public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltShield, 1)
                .AddIngredient<CornerGrass>() // 修正为正确的ModItem名称
                .AddIngredient(ItemID.FeralClaws, 1)
                .AddTile(TileID.TinkerersWorkbench) // 工匠作坊
                .Register();
        }
    }
}