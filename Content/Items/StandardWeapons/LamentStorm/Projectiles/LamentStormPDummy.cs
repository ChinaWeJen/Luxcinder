using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Biomes;
using System;
using Luxcinder.Content.Buffs;
using Terraria.Audio;
using Luxcinder.Content.Items.StandardWeapons.LamentStorm;

namespace Luxcinder.Content.Items.StandardWeapons.LamentStorm.Projectiles
{
    public class LamentStormPDummy : ModProjectile
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
            Projectile.timeLeft = 40;
			Projectile.aiStyle = -1;
            Projectile.extraUpdates = 5;
            Projectile.alpha = 128; // 半透明效果
            Projectile.velocity *= 0.66f; // 速度减慢三分之一
            Projectile.noDropItem = true;
			Projectile.tileCollide = false;
            SoundEngine.PlaySound(SoundID.Item5, Projectile.position); // 射击音效
        }

		public override bool? CanDamage() => false;

        public override void AI()
        {
            // 发光效果
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.8f);

			Projectile.velocity.Y += 0.14f;

			// 旋转效果
			Projectile.rotation = Projectile.velocity.ToRotation();
            
            // 拖尾粒子
            if (Main.rand.NextBool(3))
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Vector2.Zero, 0, default, 1.5f);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * 0.2f;
            }
        }

		public override void Kill(int timeLeft)
		{
			CreateFallingArrows(Projectile.damage, Projectile.owner);
		}

        public void CreateFallingArrows(int damage, int owner)
        {
            for (int i = 0; i < 1; i++)
            {
                var velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f), // 水平散射
                    Main.rand.NextFloat(8f, 14f)   // 下落速度
                ) * 1.5f;
                
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    new Vector2(Projectile.ai[0] + Main.rand.Next(-50, 50), Projectile.ai[1] - 1200),
                    velocity,
                    ModContent.ProjectileType<LamentStormP>(),
                    damage,
                    0f,
                    owner
                );
            }
        }
    }
}