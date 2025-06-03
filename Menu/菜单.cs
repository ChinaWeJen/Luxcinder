using System;
using Terraria;
using Terraria.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

using Terraria.ID;
using Terraria.ModLoader;

namespace ErosionCinder.Menu
{
    
    public class ErosionCinderModMenu : ModMenu
    {
        // 新增：记录上一帧的鼠标位置
        private Vector2 lastMousePosition = Vector2.Zero;
        // 新增：粒子生成计数器
        private int particleCooldown = 0;
        private const int particleCooldownMax = 2; // 控制粒子生成速度

        public class Cinder
        {
            public int Time;
            public int Lifetime;
            public int IdentityIndex;
            public float Scale;
            public float Depth;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;

            public Cinder(int lifetime, int identity, float depth, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                IdentityIndex = identity;
                Depth = depth;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
            }
        }

        public class SmokeParticle
        {
            public int Time;
            public int Lifetime;
            public float Scale;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;

            public SmokeParticle(int lifetime, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
                Scale = Main.rand.NextFloat(0.5f, 1.5f);
            }
        }

        public static List<Cinder> Cinders
        {
            get;
            internal set;
        } = new();

        public static List<SmokeParticle> SmokeParticles
        {
            get;
            internal set;
        } = new();

        public override string DisplayName => "ErosionCinder";

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("ErosionCinder/Menu/标题");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("ErosionCinder/Menu/空洞像素");
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("ErosionCinder/Menu/空洞像素");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NullSurfaceBackground>();
        public override void OnSelected()
        {
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Zombie93); // 选择了这个ModMenu之后播放一个打雷音效
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sound/Music/ZCD/ZCDYY");
        // 设置音乐音量 (1.0f是最大音量)


        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            // 在鼠标位置生成红色粒子 (修改为检测鼠标移动)
            if (Main.MouseScreen != lastMousePosition) // 检测鼠标是否移动
            {
                // 限制粒子生成速度，避免过多粒子
                if (particleCooldown <= 0)
                {
                    // 计算鼠标移动速度，速度越快生成的粒子越多
                    float moveSpeed = Vector2.Distance(Main.MouseScreen, lastMousePosition);
                    int particleCount = Math.Max(1, (int)(moveSpeed * 0.1f));

                    for (int i = 0; i < particleCount; i++)
                    {
                        int lifetime = Main.rand.Next(30, 100);
                        float depth = Main.rand.NextFloat(1f, 3f);
                        // 在鼠标位置周围随机分布粒子
                        Vector2 startingPosition = Main.MouseScreen + new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                        // 粒子速度与鼠标移动方向相关
                        Vector2 mouseDirection = (Main.MouseScreen - lastMousePosition).SafeNormalize(Vector2.Zero);
                        Vector2 startingVelocity = mouseDirection * Main.rand.NextFloat(0.5f, 2f) +
                                                   new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));

                        // 生成渐变红色粒子
                        Color redColor = Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat());
                        Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, redColor, startingPosition, startingVelocity));
                    }

                    particleCooldown = particleCooldownMax;
                }
                else
                {
                    particleCooldown--;
                }

                lastMousePosition = Main.MouseScreen; // 更新上一帧鼠标位置
            }

            Texture2D texture = ModContent.Request<Texture2D>("ErosionCinder/Menu/背景").Value;
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / texture.Width;
            float yScale = (float)Main.screenHeight / texture.Height;
            float scale = xScale;
            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (texture.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                    drawOffset.Y -= (texture.Height * scale - Main.screenHeight) * 0.5f;
            }
            spriteBatch.Draw(texture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            static Color selectCinderColor()
            {
                return Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.9f));
            }

            // 生成粒子（减少数量）
            for (int i = 0; i < 2; i++) // 从 5 减少到 2
            {
                if (Main.rand.NextBool(8)) // 降低生成概率，从 4 改为 8
                {
                    int lifetime = Main.rand.Next(200, 300);
                    float depth = Main.rand.NextFloat(1.8f, 5f);
                    Vector2 startingPosition = new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                    Vector2 startingVelocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f)) * 4f;
                    Color cinderColor = selectCinderColor();
                    // 降低透明度
                    cinderColor *= 0.5f;
                    Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, cinderColor, startingPosition, startingVelocity));
                }
            }

            // 生成烟雾粒子（减少数量）
            for (int i = 0; i < 1; i++) // 从 3 减少到 1
            {
                if (Main.rand.NextBool(20)) // 降低生成概率，从 10 改为 20
                {
                    int lifetime = Main.rand.Next(100, 200);
                    Vector2 startingPosition = new Vector2(Main.screenWidth * Main.rand.NextFloat(0.1f, 0.9f), Main.screenHeight);
                    Vector2 startingVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-2f, -1f));
                    Color smokeColor = new Color(100, 100, 100, 100);
                    // 降低透明度
                    smokeColor *= 0.4f;
                    SmokeParticles.Add(new SmokeParticle(lifetime, smokeColor, startingPosition, startingVelocity));
                }
            }

            // 在鼠标位置生成红色粒子
            if (Main.mouseLeft) // 可以根据需要修改触发条件，这里是鼠标左键按下时生成粒子
            {
                for (int i = 0; i < 3; i++)
                {
                    int lifetime = Main.rand.Next(50, 150);
                    float depth = Main.rand.NextFloat(1f, 3f);
                    Vector2 startingPosition = Main.MouseScreen;
                    Vector2 startingVelocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Color redColor = Color.Red;
                    Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, redColor, startingPosition, startingVelocity));
                }
            }

            // 更新粒子
            for (int i = 0; i < Cinders.Count; i++)
            {
                Cinders[i].Scale = Terraria.Utils.GetLerpValue(Cinders[i].Lifetime, Cinders[i].Lifetime / 3, Cinders[i].Time, true);
                Cinders[i].Scale *= MathHelper.Lerp(0.6f, 0.9f, Cinders[i].IdentityIndex % 6f / 6f);
                if (Cinders[i].IdentityIndex % 13 == 12)
                    Cinders[i].Scale *= 2f;
                Cinders[i].Scale *= 1.5f; // 增大粒子尺寸

                float flySpeed = MathHelper.Lerp(3.2f, 14f, Cinders[i].IdentityIndex % 21f / 21f);
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.44f, 0.44f, (float)Math.Sin(Cinders[i].Time / 16f + Cinders[i].IdentityIndex) * 0.5f + 0.5f));
                idealVelocity = (idealVelocity + Vector2.UnitX).SafeNormalize(Vector2.UnitY) * flySpeed;

                float movementInterpolant = MathHelper.Lerp(0.01f, 0.08f, Terraria.Utils.GetLerpValue(45f, 145f, Cinders[i].Time, true));
                Cinders[i].Velocity = Vector2.Lerp(Cinders[i].Velocity, idealVelocity, movementInterpolant);

                Cinders[i].Time++;
                Cinders[i].Center += Cinders[i].Velocity;
            }

            // 更新烟雾粒子
            for (int i = 0; i < SmokeParticles.Count; i++)
            {
                SmokeParticles[i].Time++;
                SmokeParticles[i].Center += SmokeParticles[i].Velocity;
                SmokeParticles[i].Scale += 0.01f;
                SmokeParticles[i].DrawColor = new Color(SmokeParticles[i].DrawColor.R, SmokeParticles[i].DrawColor.G, SmokeParticles[i].DrawColor.B, (int)(255 * (1 - (float)SmokeParticles[i].Time / SmokeParticles[i].Lifetime)));
            }

            // 删除粒子
            Cinders.RemoveAll(c => c.Time >= c.Lifetime);
            SmokeParticles.RemoveAll(s => s.Time >= s.Lifetime);

            // 绘制粒子
            Texture2D cinderTexture = ModContent.Request<Texture2D>("ErosionCinder/Menu/粒子").Value;
            for (int i = 0; i < Cinders.Count; i++)
            {
                Vector2 drawPosition = Cinders[i].Center;
                spriteBatch.Draw(cinderTexture, drawPosition, null, Cinders[i].DrawColor, 0f, cinderTexture.Size() * 0.5f, Cinders[i].Scale, 0, 0f);
            }

            // 绘制烟雾粒子
            Texture2D smokeTexture = ModContent.Request<Texture2D>("ErosionCinder/Menu/烟雾").Value;
            for (int i = 0; i < SmokeParticles.Count; i++)
            {
                Vector2 drawPosition = SmokeParticles[i].Center;
                spriteBatch.Draw(smokeTexture, drawPosition, null, SmokeParticles[i].DrawColor, 0f, smokeTexture.Size() * 0.5f, SmokeParticles[i].Scale, 0, 0f);
            }

            // 时间and标题
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            // 绘制标题
            logoScale *= 2f; // 增大标题尺寸
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(Logo.Value, drawPos, null, drawColor, logoRotation, Logo.Value.Size() * 0.5f, logoScale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            return false;
        }
    }
}