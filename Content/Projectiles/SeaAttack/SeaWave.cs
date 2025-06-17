using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace Luxcinder.Content.Projectiles.SeaAttack
{
    public class SeaWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            Main.projFrames[Projectile.type] = 1; // 5帧动画
        }

        public override void SetDefaults()
        {
            // 基础属性
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            
            // 持续时间
            Projectile.timeLeft = 180;
            
            // 视觉效果
            Projectile.scale = 1.3f;
            Projectile.alpha = 60;
        }

        public override void AI()
        {
            // 动画效果

            
            // 旋转效果

            
            // 逐渐减速
            if (Projectile.timeLeft < 150)
            {
                Projectile.velocity *= 0.97f;
            }
            
            // 水波粒子效果
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                            DustID.Water, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 
                            100, default, 1.7f);
            }
            
            // 光照效果
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 自定义绘制
            Texture2D texture = ModContent.Request<Texture2D>("Luxcinder/Content/Projectiles/SeaAttack/SeaWave").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, 
                                frame, Color.White * 0.85f, Projectile.rotation, 
                                frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            
            return false;
        }

public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 命中效果
            target.AddBuff(BuffID.Wet, 240); // 4秒潮湿效果
            
            // 水花爆发效果
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, 
                            DustID.Water, 0f, 0f, 100, default, 1.8f);
            }
            
            // 溅水音效
            SoundEngine.PlaySound(SoundID.SplashWeak, Projectile.position);
        }
    }
}