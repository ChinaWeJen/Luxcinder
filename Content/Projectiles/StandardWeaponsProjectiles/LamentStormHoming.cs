using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace Luxcinder.Content.Projectiles.StandardWeaponsProjectiles
{
    public class LamentStormHoming : ModProjectile
    {
        private NPC target;
        private bool IsLightning => Projectile.ai[0] == 1;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 100;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 视觉效果
            Lighting.AddLight(Projectile.Center, IsLightning ? 0.5f : 0.1f, IsLightning ? 0.8f : 0.4f, IsLightning ? 1f : 0.8f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 粒子效果
            int dustType = IsLightning ? DustID.Electric : DustID.Cloud;
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, Vector2.Zero, 0, default, 1.5f);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.2f;
            }

            // 寻找目标
            if (target == null || !target.active || target.Distance(Projectile.Center) > 300f)
            {
                FindTarget();
            }

            // 追踪目标
            if (target != null && target.active)
            {
                Vector2 direction = target.Center - Projectile.Center;
                float distance = direction.Length();
                direction.Normalize();

                // 在80px范围内才开始精确追踪
                if (distance < 80f)
                {
                    float speed = Projectile.velocity.Length();
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * speed, 0.1f);
                }
            }
        }

        private void FindTarget()
        {
            float closestDistance = 300f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < closestDistance)
                {
                    closestDistance = npc.Distance(Projectile.Center);
                    target = npc;
                }
            }
        }

public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)        {
            // 雷属性连锁伤害
            if (IsLightning)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc != target && npc.Distance(target.Center) < 100f)
                    {
    int reducedDamage = (int)(hit.Damage * 0.6f);
        float reducedKnockback = hit.Knockback * 0.5f;

                        
                        // 闪电效果
                        for (int j = 0; j < 3; j++)
                        {
                            Dust dust = Dust.NewDustPerfect(
                                Vector2.Lerp(Projectile.Center, npc.Center, j / 3f),
                                DustID.Electric,
                                Vector2.Zero,
                                0,
                                Color.White,
                                1.5f
                            );
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }
    }
}