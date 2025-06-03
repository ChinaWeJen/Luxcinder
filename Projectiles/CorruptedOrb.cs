using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.Audio;

namespace ErosionCinder.Projectiles
{
    public class CorruptedOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 动画效果
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // 粒子效果
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.PurpleTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            // 平滑跟踪逻辑
            float maxDetectRadius = 600f;
            float projSpeed = 12f;
            float lerpFactor = 0.1f; // 转向平滑度

            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC != null)
            {
                Vector2 desiredVelocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, lerpFactor);
                
                // 靠近目标时加速
                float distanceToTarget = Vector2.Distance(Projectile.Center, closestNPC.Center);
                if (distanceToTarget < 200f)
                {
                    Projectile.velocity *= 1.05f;
                }
            }
            
            // 旋转效果
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            // 更丰富的粒子效果
            if (Main.rand.NextBool(2))
            {
                int dustType = Main.rand.NextBool() ? DustID.PurpleTorch : DustID.PinkTorch;
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    dustType, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 150, default, 1.4f);
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override void OnKill(int timeLeft)
        {
            // 死亡时产生爆炸效果
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            
            for (int i = 0; i < 20; i++)
            {
                // 紫色粒子
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.PurpleTorch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.6f;
                dust.noGravity = true;
                
                // 粉色粒子
                if (i % 3 == 0)
                {
                    Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                        DustID.PinkTorch, 0f, 0f, 100, default, 1.8f);
                    dust2.velocity *= 1.2f;
                    dust2.noGravity = true;
                }
            }
            
            // 爆炸光效
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, speed * 5, Scale: 2.5f);
                d.noGravity = true;
            }
        }
    }
}