﻿﻿﻿using Terraria;
﻿using Terraria.GameContent.Creative;
﻿using Terraria.ID;
﻿using Terraria.ModLoader;

﻿namespace Luxcinder.Content.Items.Accessory
﻿{
﻿    /// <summary>
﻿    /// 塔尔塔罗斯之盾 - 提供生命值、防御和近战增强效果
﻿    /// </summary>
﻿    public class TartarusShield : ModItem
﻿    {
﻿        public override void SetStaticDefaults() 
﻿        {
﻿            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
﻿            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // 影响物品栏排序(59=铂金锭，数值越高越靠后)
﻿        }

﻿        public override void SetDefaults() 
﻿        {
﻿            // 基础属性
﻿            Item.width = 20;
﻿            Item.height = 20;
﻿            Item.maxStack = 1;
﻿            Item.rare = ItemRarityID.LightRed;
﻿            Item.defense = 4;
            
﻿            // 使用属性
﻿            Item.useStyle = ItemUseStyleID.Swing;
﻿            Item.useTime = 10;
﻿            Item.useAnimation = 15;
﻿            Item.useTurn = true;
﻿            Item.autoReuse = true;
﻿            Item.placeStyle = 0;
            
﻿            // 饰品属性
﻿            Item.accessory = true;
            
﻿            // 价值(35金币)
﻿            Item.value = Item.sellPrice(gold: 35);
﻿        }

﻿        /// <summary>
﻿        /// 装备效果：
﻿        /// - 增加生命上限和生命恢复
﻿        /// - 免疫击退和燃烧效果
﻿        /// - 增强近战能力
﻿        /// - 高生命值时获得额外防御和伤害
﻿        /// </summary>
﻿        public override void UpdateAccessory(Player player, bool hideVisual) 
﻿        {
﻿            // 基础效果
﻿            player.statLifeMax2 += 100;
﻿            player.lifeRegen += 1;
﻿            player.noKnockback = true;
            
﻿            // 免疫效果
﻿            player.buffImmune[BuffID.Burning] = true;
﻿            player.buffImmune[BuffID.OnFire] = true;
            
﻿            // 近战增强
﻿            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
﻿            player.GetCritChance(DamageClass.Melee) += 0.07f;
﻿            player.GetDamage(DamageClass.Melee) += 0.5f;
            
﻿            // 高生命值奖励
﻿            if (player.statLifeMax2 >= 350)
﻿            {
﻿                player.statDefense += 4;
﻿                player.GetDamage(DamageClass.Generic) += 0.10f;
﻿            }
﻿        }

﻿        public override void AddRecipes() 
﻿        {
﻿            CreateRecipe()

﻿                .AddIngredient(ItemID.HellstoneBar, 25)
﻿                .AddIngredient(ItemID.MeteoriteBar, 15)
﻿                .AddIngredient(ItemID.ObsidianBrick, 50)
﻿                .AddTile(TileID.Hellforge) // 地狱熔炉
﻿                .Register();
﻿        }
﻿    }
﻿}
