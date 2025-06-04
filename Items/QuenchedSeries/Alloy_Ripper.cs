﻿﻿﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Luxcinder.Items.QuenchedSeries
{

    // 合金撕裂者武器类
    public class Alloy_Ripper : ModItem
    {

        public override void SetDefaults()
        {
            // 基础属性设置（符合用户需求）
            Item.damage = 26;  // 基础伤害(原29降低10.3%)
            Item.DamageType = DamageClass.Melee;  // 近战伤害类型
            Item.width = 46;  // 物品宽度（像素）
            Item.height = 46;  // 物品高度（像素）
            Item.useTime = 22;  // 使用时间（保持不变）
            Item.useAnimation = 22;  // 动画持续时间（需与useTime一致）
            Item.useStyle = ItemUseStyleID.Swing;  // 使用方式：挥舞
            Item.knockBack = 5f;  // 击退力(原5.5降低9.1%)
            Item.value = Item.buyPrice(gold: 12);  // 物品价值（12金）
            Item.rare = ItemRarityID.LightPurple;  // 稀有度：浅紫色
            Item.UseSound = SoundID.Item1;  // 使用音效（剑类通用）
            Item.autoReuse = true;  // 自动连续攻击
            Item.crit = 12;  // 基础暴击率(原15降低至12%)
        }
        // 自定义增益Buff（用于防御<35时的血量加成）
        public class Alloy_Ripper_Buff : ModBuff
        {
            public override void SetStaticDefaults()
            {

                Main.buffNoSave[Type] = true;  // 不保存到存档
                Main.buffNoTimeDisplay[Type] = true;  // 不显示剩余时间
            }

            public override void Update(Player player, ref int buffIndex)
            {
                player.statLifeMax2 += 100;  // 直接增加最大生命上限
                //if (player.statLife > player.statLifeMax2)  // 防止生命溢出
                //    player.statLife = player.statLifeMax2;
            }
        }

        public override void HoldItem(Player player)
        {
            player.statLifeMax += 100;
            // 仅当玩家防御<35时触发
            if (player.statDefense < 35)
            {
                // 添加自定义Buff（持续2帧，每帧刷新以保持效果）
                player.AddBuff(ModContent.BuffType<Alloy_Ripper_Buff>(), 2);

            }

        }


        // 关键逻辑1：防御<35时的血量加成（持有武器时生效）
        public override void UpdateEquip(Player player)
        {
            // 仅当玩家防御<35时触发
            if (player.statDefense < 35)
            {
                // 添加自定义Buff（持续2帧，每帧刷新以保持效果）
                player.AddBuff(ModContent.BuffType<Alloy_Ripper_Buff>(), 2);
                
            }
            else
            {
                // 防御≥35时移除Buff
                int buffIndex = player.FindBuffIndex(ModContent.BuffType<Alloy_Ripper_Buff>());
                if (buffIndex != -1)
                    player.DelBuff(buffIndex);
            }
        }

        // 关键逻辑2：切换武器时移除增益（确保效果仅在持有时生效）
        public override void UpdateInventory(Player player)
        {
            // 如果当前未持有该武器（在背包中），移除Buff
            if (player.HeldItem.type != Item.type)
            {
                int buffIndex = player.FindBuffIndex(ModContent.BuffType<Alloy_Ripper_Buff>());
                if (buffIndex != -1)
                    player.DelBuff(buffIndex);
            }
        }

        // 关键逻辑3：击中敌人附加燃烧效果+火焰粒子
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 附加6秒燃烧效果（6*60=360帧）
            target.AddBuff(BuffID.OnFire, 360);

            // 生成火焰喷射粒子（击中时特效）
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2.5f, 2.5f),  // X方向速度
                    Main.rand.NextFloat(-2.5f, 2.5f)   // Y方向速度
                );
                Dust.NewDustPerfect(
                    target.Center,  // 粒子生成位置（NPC中心）
                    DustID.Firefly,    // 火焰粒子类型
                    velocity,       // 粒子速度
                    Alpha: 50,      // 透明度（50/255）
                    new Color(255, 150, 50),  // 橙红色火焰
                    Scale: 1.3f     // 粒子大小（1.3倍）
                );
            }
        }

        // 关键逻辑4：攻击时的火焰粒子（挥舞武器时特效）
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // 30%概率生成火焰粒子（覆盖攻击碰撞箱）
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    new Vector2(hitbox.X, hitbox.Y),  // 碰撞箱起点
                    hitbox.Width, hitbox.Height,       // 碰撞箱尺寸
                    DustID.Firefly,                       // 火焰粒子
                    0f, 0f,                            // 默认速度
                    Alpha: 80,                         // 半透明
                    new Color(255, 180, 100),          // 亮橙红色
                    Scale: 1.1f                        // 粒子大小
                );
                dust.noGravity = true;  // 无重力（漂浮效果）
                dust.velocity *= 0.6f;  // 速度减缓
            }
        }

        // 合成配方（示例）
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 18)  // 狱岩锭×18
                .AddIngredient(ItemID.SoulofMight, 8)  // 力量之魂×8
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}