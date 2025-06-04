using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Projectiles
{
    public class AlloyFire : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 50;
        }

        public override void AI()
        {
            // 火焰粒子效果
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
            
            // 抛物线运动
            Projectile.velocity.Y += 0.2f; // 重力效果
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)        {
            // 命中时产生爆炸效果
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Firework_Red, 0f, 0f, 100, default, 1.5f);
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            // 消失时产生小爆炸
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }
    }
}