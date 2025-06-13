using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Projectiles
{
    public class GoldenLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;  // 较粗的激光
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180; // 3秒持续时间
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 1f; // 发光效果
            Projectile.velocity *= 2.5f; // 提高移动速度
        }

        public override void AI()
        {
            // 金色发光效果
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0f);
            
            // 产生金色粒子
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.GoldFlame, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);
            }
            
            // 保持直线飞行
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            // 死亡时产生金色爆炸效果
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }
    }
}