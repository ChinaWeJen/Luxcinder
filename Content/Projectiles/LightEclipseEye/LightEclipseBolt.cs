using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Luxcinder.Content.Projectiles.LightEclipseEye
{
    public class LightEclipseBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.2f;
            Projectile.rotation = Projectile.velocity.ToRotation(); // 确保初始方向正确
        }

        public override void AI()
        {
            // 神圣光效
            Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0.5f);
            
            // 旋转效果
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 尾迹粒子
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            // 智能追踪(第三阶段)
            if (Projectile.ai[0] == 1 && Projectile.timeLeft < 250)
            {
                float maxSpeed = 12f;
                float homingStrength = 0.1f;
                
                NPC target = Main.npc[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                if (target != null && target.active)
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                    Projectile.velocity = (Projectile.velocity * (1f - homingStrength) + desiredVelocity * homingStrength).SafeNormalize(Vector2.Zero) * maxSpeed;
                }
            }
        }

        [Obsolete]
        public override void OnKill(int timeLeft)
        {
            // 爆炸效果
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.GoldFlame, 0f, 0f, 100, default, 2f);
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
        }
    }
}