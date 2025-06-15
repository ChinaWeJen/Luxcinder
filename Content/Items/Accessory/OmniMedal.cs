﻿﻿﻿using Terraria;
﻿﻿using Terraria.GameContent.Creative;
﻿﻿using Terraria.ID;
﻿﻿using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Accessory
﻿﻿{
﻿﻿    /// <summary>
﻿﻿    /// 全能者勋章 - 提供全职业伤害加成和生命恢复
﻿﻿    /// </summary>
﻿﻿    public class OmniMedal : ModItem
﻿﻿    {
﻿﻿        public override void SetStaticDefaults() 
﻿﻿        {
﻿﻿            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
﻿﻿            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // 影响物品栏排序(59=铂金锭，数值越高越靠后)
﻿﻿        }

﻿﻿        public override void SetDefaults() 
﻿﻿        {
﻿﻿            // 基础属性
﻿﻿            Item.width = 20;
﻿﻿            Item.height = 20;
﻿﻿            Item.maxStack = 1;
﻿﻿            Item.rare = ItemRarityID.Blue;
﻿﻿            Item.defense = 2;
            
﻿﻿            // 使用属性
﻿﻿            Item.useStyle = ItemUseStyleID.Swing;
﻿﻿            Item.useTime = 10;
﻿﻿            Item.useAnimation = 15;
﻿﻿            Item.useTurn = true;
﻿﻿            Item.autoReuse = true;
﻿﻿            Item.placeStyle = 0;
            
﻿﻿            // 饰品属性
﻿﻿            Item.accessory = true;
            
﻿﻿            // 价值(35金币)
﻿﻿            Item.value = Item.sellPrice(gold: 35);
﻿﻿        }

﻿﻿        /// <summary>
﻿﻿        /// 装备效果：
﻿﻿        /// - 增加9%全伤害
﻿﻿        /// - 提升生命恢复速度
﻿﻿        /// - 免疫击退
﻿﻿        /// </summary>
﻿﻿        public override void UpdateAccessory(Player player, bool hideVisual) 
﻿﻿        {
﻿﻿            player.GetDamage(DamageClass.Generic) += 0.09f;
﻿﻿            player.lifeRegen += 1;
﻿﻿            player.noKnockback = true;
﻿﻿        }

﻿﻿        public override void AddRecipes() 
﻿﻿        {
﻿﻿            CreateRecipe()
﻿﻿                .AddIngredient(ItemID.WarriorEmblem)
﻿﻿                .AddIngredient(ItemID.RangerEmblem)
﻿﻿                .AddIngredient(ItemID.SorcererEmblem)
﻿﻿                .AddIngredient(ItemID.SoulofLight, 5)
﻿﻿                .AddIngredient(ItemID.SoulofNight, 5)
﻿﻿                .AddTile(TileID.TinkerersWorkbench) // 工匠作坊
﻿﻿                .Register();
﻿﻿        }
﻿﻿    }
﻿﻿}