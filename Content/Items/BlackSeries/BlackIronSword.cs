﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

using System;

namespace Luxcinder.Content.Items.BlackSeries
{
    public class BlackIronSword : ModItem
    {
        // 新常量定义
        private const float SmallDamageChance = 0.8f; // 25%概率25点伤害
        private const float MediumDamageChance = 0.25f; // 25%概率25点伤害
        private const float HighDamageChance = 0.03f;    // 10%概率40点伤害
        private const int SmallDamageValue = 8;
        private const int MediumDamageValue = 26;       // 中等伤害值
        private const int HighDamageValue = 40;


        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 60; // 影响物品在材料列表中的排序位置
        }

        public override void SetDefaults()
        {
            Item.damage = 5; // 基础伤害(原18降低11.1%)
            Item.DamageType = DamageClass.Melee; // 近战伤害类型
            Item.width = 40; // 物品宽度
            Item.height = 40; // 物品高度
            Item.useTime = 20; // 使用时间（与原版铂金剑一致）
            Item.useAnimation = 20; // 动画时间
            Item.useStyle = ItemUseStyleID.Swing; // 使用方式：挥舞
            Item.knockBack = 5; // 击退效果(原6降低16.7%)
            Item.value = Item.buyPrice(gold: 5); // 物品价值（5金）
            Item.rare = ItemRarityID.LightRed; // 物品稀有度：浅红色
            Item.UseSound = SoundID.Item1; // 使用音效
            Item.autoReuse = true; // 自动连续使用
            Item.crit = 10; // 暴击率(原13降低至10%)
        }

        // 修改伤害计算
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            int baseDamage = Item.damage;
            float randomChance = Main.rand.NextFloat();

            // 优先判断高伤害效果
            if (randomChance < HighDamageChance)
            {
                Item.damage = HighDamageValue;
                modifiers.SetCrit(); // 强制触发暴击特效
            }
            // 不触发高伤害时，判断中等伤害效果
            else if (randomChance < MediumDamageChance)
            {
                Item.damage = MediumDamageValue;
                modifiers.SetCrit(); // 强制触发暴击特效
            }
            // 不触发中等伤害时，判断小伤害效果
            else if (randomChance < SmallDamageChance)
            {
                Item.damage = SmallDamageValue;
                modifiers.SetCrit(); // 强制触发暴击特效
            }
            else
            {
                Item.damage = baseDamage;
            }
        }

        // 关键方法2：命中NPC后的特效（仅客户端生效）
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 需求3：暴击时生成血色粒子喷射
            if (hit.Crit) // 检查是否为暴击（基于ModifyHitNPC中的crit参数）
            {
                // 生成15个血色粒子
                for (int i = 0; i < 15; i++)
                {
                    // 随机粒子速度（X/Y方向-3到3）
                    Vector2 velocity = new Vector2(
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-3f, 3f)
                    );

                    // 生成粒子（使用DustID.Blood，血色粒子）
                    Dust dust = Dust.NewDustDirect(
                        target.position, // 粒子生成位置（NPC碰撞箱起点）
                        target.width,    // NPC宽度（粒子覆盖范围）
                        target.height,   // NPC高度（粒子覆盖范围）
                        DustID.Blood,    // 粒子类型（血色）
                        velocity.X,      // X方向速度
                        velocity.Y,      // Y方向速度
                        Alpha: 50,       // 透明度（0=完全透明，255=不透明）
                        new Color(255, 50, 50), // 粒子颜色（可调整）
                        Scale: 1.2f      // 粒子大小（1.2倍默认）
                    );

                    dust.noGravity = true; // 粒子不受重力影响（漂浮）
                }
            }
        }

        // 合成配方（示例）
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackIronIngot>(12) // 自定义材料：黑铁锭
                .AddIngredient(ItemID.MeteoriteBar, 8) // 原版材料：陨铁锭
                .AddTile(TileID.Anvils) // 合成站：铁砧/铅砧
                .Register();
        }
    }
}