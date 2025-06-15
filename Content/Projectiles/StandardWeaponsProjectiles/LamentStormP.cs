using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Biomes;
using System;
using Luxcinder.Content.Items.StandardWeapons;
using Luxcinder.Content.Buffs;
using Terraria.Audio;

namespace Luxcinder.Content.Projectiles.StandardWeaponsProjectiles
{
    public class LamentStormP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // 无限穿透
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 128; // 半透明效果
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.velocity *= 0.66f; // 速度减慢三分之一
            Projectile.aiStyle = 1; // 使用标准抛射物AI
            Projectile.noDropItem = true;
            SoundEngine.PlaySound(SoundID.Item5, Projectile.position); // 射击音效
        }

        public override void AI()
        {
            // 发光效果
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.8f);
            
            // 旋转效果
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            // 抛物线效果 - 添加重力
            Projectile.velocity.Y += 0.2f; // 重力加速度
            
            // 拖尾粒子
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Vector2.Zero, 0, default, 1.5f);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.2f;
            }
        }

        public static void CreateFallingArrows(Vector2 targetPosition, int damage, int owner)
        {
            int arrowCount = Main.rand.Next(3, 6); // 3-5只箭
            for (int i = 0; i < arrowCount; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f), // 水平散射
                    Main.rand.NextFloat(5f, 8f)   // 下落速度
                );
                
                Projectile.NewProjectile(
                    Projectile.GetSource_NaturalSpawn(),
                    new Vector2(targetPosition.X + Main.rand.Next(-50, 50), targetPosition.Y - 600),
                    velocity,
                    ModContent.ProjectileType<LamentStormP>(),
                    damage,
                    0f,
                    owner
                );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 获取武器实例
            if (Main.player[Projectile.owner].HeldItem.ModItem is LamentStorm bow)
            {
                // 暴击时产生旋风
                if (hit.Crit)
                {
                    SpawnVortex(target.Center, damageDone);
                }

                // 增加风之印记
                if (bow.GetWindSigils() < (Main.raining ? 15 : 10))
                {
                    bow.IncrementWindSigils();
                }

                // 环境效果
                if (Main.raining && target.wet)
                {
                    target.AddBuff(BuffID.Electrified, 180);
                }
                if (Main.hardMode && Main.SceneMetrics.SnowTileCount > 0)
                {
                    target.AddBuff(ModContent.BuffType<Frostbite>(), 180);
                }
                if (target.position.Y > Main.maxTilesY * 16 - 3000) // Underworld check
                {
                    target.AddBuff(BuffID.OnFire, 180);
                }
            }

            // 伤害衰减：每次穿透后伤害减少25%
            Projectile.damage = (int)(Projectile.damage * 0.75f);
            if (Projectile.damage < 1)
            {
                Projectile.damage = 1; // 最低伤害为1
            }
        }

        private void SpawnVortex(Vector2 position, int damage)
        {
            // 创建吸引敌人的小型旋风
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                position,
                Vector2.Zero,
                ModContent.ProjectileType<LamentStormVortex>(),
                (int)(damage * 0.2f), // 20%武器伤害
                0f,
                Projectile.owner
            );
        }
    }

    public class LamentStormVortex : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60; // 存在1秒
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
        }

        public override void AI()
        {
            // 吸引附近敌人
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < 200f)
                {
                    Vector2 pullDirection = Projectile.Center - npc.Center;
                    float distance = pullDirection.Length();
                    pullDirection.Normalize();
                    npc.velocity = pullDirection * (10f * (1f - distance / 200f));
                }
            }

            // 漩涡效果
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(40, 40),
                    DustID.Cloud,
                    Vector2.Zero,
                    0,
                    default,
                    2f
                );
                dust.noGravity = true;
                dust.velocity = (dust.position - Projectile.Center).RotatedBy(MathHelper.ToRadians(10)) * 0.1f;
            }
        }
    }
}