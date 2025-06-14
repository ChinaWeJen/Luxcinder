using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Luxcinder.Content.Projectiles.Bosses
{
    public class GoddessDimLightAttack : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            // 确保不伤害Boss
            if (Projectile.localNPCImmunity[Main.npc[Projectile.owner].whoAmI] == 0)
            {
                Projectile.localNPCImmunity[Main.npc[Projectile.owner].whoAmI] = 1;
            }

            // 光女风格的粒子效果
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, 
                    Vector2.Zero, 0, default, 1.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }

            // 添加蓝色光效粒子
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, 
                    Projectile.velocity * 0.2f, 0, default, 1.8f);
                dust.noGravity = true;
            }

            // 流星拖尾效果
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i * 0.5f, 
                    DustID.PurpleTorch, Vector2.Zero);
                dust.noGravity = true;
                dust.scale = 1.2f - i * 0.2f;
                dust.alpha = 100 + i * 50;
            }

            // 旋转效果
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            // 逐渐显现实心核心
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制拖尾
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - (float)i / Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.Purple, Color.Blue, progress) * progress;
                float scale = Projectile.scale * progress * 0.8f;
                
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    Main.EntitySpriteDraw(
                        ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion).Value,
                        Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition,
                        null,
                        color,
                        Projectile.rotation,
                        new Vector2(50, 50),
                        scale,
                        SpriteEffects.None,
                        0);
                }
            }
            
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // 击中时的爆炸效果
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 6f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.PurpleTorch : DustID.BlueTorch, velocity);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.5f, 2.5f);
            }
            
            // 光效
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, 
                    Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 4f));
                dust.noGravity = true;
                dust.scale = 1.8f;
            }
        }
    }
}