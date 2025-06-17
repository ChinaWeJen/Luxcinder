using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.StandardWeapons.PureEdge
{
    public class PureEdgeSlash : ModProjectile
    {
        public override string Texture => "Luxcinder/Common/Empty";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6; // 用于动画效果
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 45;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            // 动画帧控制
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            float progress = Projectile.timeLeft / 45f; // 生命周期进度(1->0)
            float intensity = 1f - progress; // 强度(0->1)

            // 核心能量粒子 - 蓝色电光
            for (int i = 0; i < 4; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(60, 60) * intensity;
                Dust.NewDustPerfect(
                    position,
                    DustID.Electric,
                    Main.rand.NextVector2Circular(2, 2),
                    0,
                    Color.Lerp(Color.Cyan, Color.White, intensity),
                    1.2f * intensity
                );
            }

            // 能量光环 - 蓝色光晕
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Firework_Blue,
                    Main.rand.NextVector2Circular(4, 4),
                    0,
                    new Color(100, 255, 255, 100),
                    1.8f * intensity
                );
            }

            // 能量火花 - 白色闪光
            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(40, 40),
                    DustID.WhiteTorch,
                    Main.rand.NextVector2Circular(3, 3),
                    0,
                    Color.White,
                    1.5f * intensity
                );
            }

            // 能量轨迹 - 跟随运动
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueFairy,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    new Color(0, 200, 255, 100),
                    1.5f * intensity
                );
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false; // 不绘制任何贴图
        }
    }
}