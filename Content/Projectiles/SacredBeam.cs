// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Projectiles
// {
//     public class SacredBeam : ModProjectile
//     {
//         public override void SetStaticDefaults()
//         {
//             DisplayName.SetDefault("神圣光束");
//         }

//         public override void SetDefaults()
//         {
//             Projectile.width = 8;
//             Projectile.height = 8;
//             Projectile.friendly = true;
//             Projectile.DamageType = DamageClass.Magic;
//             Projectile.penetrate = 3;
//             Projectile.timeLeft = 120;
//             Projectile.extraUpdates = 100;
//             Projectile.ignoreWater = true;
//             Projectile.tileCollide = false;
//             Projectile.usesLocalNPCImmunity = true;
//             Projectile.localNPCHitCooldown = 10;
//         }

//         public override void AI()
//         {
//             // 金色光效
//             Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0.3f);
            
//             // 尾迹粒子
//             if (Main.rand.NextBool(5))
//             {
//                 Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
//                     DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);
//                 dust.noGravity = true;
//                 dust.velocity *= 0.3f;
//             }

//             // 轻微弯曲轨迹
//             Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(1));
//         }

//         public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
//         {
//             // 神圣打击特效
//             for (int i = 0; i < 5; i++)
//             {
//                 Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, 
//                     DustID.GoldFlame, 0f, 0f, 100, default, 2f);
//                 dust.noGravity = true;
//                 dust.velocity *= 3f;
//             }
//         }
//     }
// }