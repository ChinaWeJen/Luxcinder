using System;
using Terraria;
using Terraria.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent;
using ReLogic.Graphics;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

namespace Luxcinder.Content.Menu
{
    [Autoload(true)]
    public class LuxcinderModMenu : ModMenu
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

        public override string DisplayName => "Luxcinder";

        public override Asset<Texture2D> Logo => this.RequestModRelativeTexture("标题");
        public override Asset<Texture2D> SunTexture => this.RequestModRelativeTexture("空洞像素");
        public override Asset<Texture2D> MoonTexture => this.RequestModRelativeTexture("空洞像素");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NullSurfaceBackground>();
        public override void OnSelected()
        {
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Zombie93); // 选择了这个ModMenu之后播放一个打雷音效
        }
            
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/ZCD/MainMenuBackgroundMusic");
        

        // 设置音乐音量 (1.0f是最大音量)


        private Asset<Texture2D> _backgroundTexture;
        private Asset<Texture2D> _cinderDustTexture;
        private Asset<Texture2D> _smokeTexture;

        public override void Load()
        {
            // 预加载纹理
            _backgroundTexture = this.RequestModRelativeTexture("背景");
            _cinderDustTexture = this.RequestModRelativeTexture("粒子");
            _smokeTexture = this.RequestModRelativeTexture("烟雾");
        }

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

            Texture2D texture = _backgroundTexture.Value;
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
            Texture2D cinderTexture = _cinderDustTexture.Value;
            for (int i = 0; i < Cinders.Count; i++)
            {
                Vector2 drawPosition = Cinders[i].Center;
                spriteBatch.Draw(cinderTexture, drawPosition, null, Cinders[i].DrawColor, 0f, cinderTexture.Size() * 0.5f, Cinders[i].Scale, 0, 0f);
            }

            // 绘制烟雾粒子
            Texture2D smokeTexture = _smokeTexture.Value;
            for (int i = 0; i < SmokeParticles.Count; i++)
            {
                Vector2 drawPosition = SmokeParticles[i].Center;
                spriteBatch.Draw(smokeTexture, drawPosition, null, SmokeParticles[i].DrawColor, 0f, smokeTexture.Size() * 0.5f, SmokeParticles[i].Scale, 0, 0f);
            }

            // 时间and标题
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            // 绘制标题 - 稳定浮动效果
            // 增强标题动画效果：增加垂直幅度并添加轻微水平晃动
            float verticalOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f) * 15f; // 增大到±15像素垂直浮动
            float horizontalOffset = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.3f) * 3.9f; // 增强到±3.9像素水平晃动
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f + horizontalOffset, 120f + verticalOffset); // 基础位置+浮动
            
            // 准备光晕效果绘制
            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Additive, // 使用Additive混合增强光晕
                SamplerState.LinearClamp, // 光晕使用线性插值
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.UIScaleMatrix
            );

            // 增强型光晕效果 - 三层渲染通道
            spriteBatch.End();
            
            // 第一通道: 基础光晕 (3倍强度)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            for (int j = 0; j < 5; j++) {
                float glowScale = 1.6f + 0.15f * (j + 1);
                float intensity = 0.9f - 0.15f * j;
                Color outerGlow = new Color(255, 80, 220, (int)(80 * intensity)) * intensity;
                Color innerGlow = new Color(180, 60, 255, (int)(100 * intensity)) * intensity;
                
                // 外光晕
                spriteBatch.Draw(Logo.Value, drawPos, null, outerGlow, 0f, Logo.Value.Size() * 0.5f, glowScale * 1.2f, SpriteEffects.None, 0f);
                // 内光晕
                spriteBatch.Draw(Logo.Value, drawPos, null, innerGlow, 0f, Logo.Value.Size() * 0.5f, glowScale, SpriteEffects.None, 0f);
            }
            
            // 第二通道: 边缘发光效果
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            for (int k = 0; k < 3; k++) {
                float edgeScale = 1.6f + 0.05f * k;
                Color edgeColor = new Color(200, 100, 255, 120) * (0.7f - 0.2f * k);
                spriteBatch.Draw(Logo.Value, drawPos, null, edgeColor, 0f, Logo.Value.Size() * 0.5f, edgeScale, SpriteEffects.None, 0f);
            }
            
            // 切换回正常绘制模式
            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp, // 标题保持锐利
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.UIScaleMatrix
            );

            // 添加轻微阴影效果
            spriteBatch.Draw(
                Logo.Value, 
                drawPos + new Vector2(2f, 2f), 
                null, 
                Color.Black * 0.5f, 
                0f,
                Logo.Value.Size() * 0.5f, 
                1.6f,
                SpriteEffects.None, 
                0f
            );
            
            // 绘制主标题
            spriteBatch.Draw(
                Logo.Value, 
                drawPos, 
                null, 
                Color.White, 
                0f,
                Logo.Value.Size() * 0.5f, 
                1.6f,
                SpriteEffects.None, 
                0f
            );
            
            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, // 保持一致性
                DepthStencilState.None, 
                Main.Rasterizer, 
                null, 
                Main.UIScaleMatrix
            );

            // 在左上角添加自定义文本
            // 多行文本内容
            string[] lines = new string[]
            {
                "感谢支持Luxcinder模组",
                "感兴趣可以进QQ群了解更多实况和更新动态",
                "我们正在努力变得更好！",
                "---Luxcinder制作组"
            };
            
            Vector2 basePosition = new Vector2(20, 20);
            float lineHeight = FontAssets.MouseText.Value.MeasureString("A").Y * 1.5f;
            
            // 动态彩色效果 - 随时间变化的颜色
            float hue = (float)(Main.timeForVisualEffects * 0.01 % 1.0);
            Color dynamicColor = Main.hslToRgb(hue, 1f, 0.75f);
            
            // 绘制每行文本
            for (int i = 0; i < lines.Length; i++)
            {
                Vector2 position = basePosition + new Vector2(0, lineHeight * i);
                Color textColor = i == lines.Length - 1 ? dynamicColor : Color.White;
                
                // 添加轻微的动态偏移效果
                if (i == lines.Length - 1)
                {
                    float offset = (float)Math.Sin(Main.timeForVisualEffects * 0.05) * 3f;
                    position.X += offset;
                    
                    // 为最后一行添加粒子光晕效果
                    if (Main.rand.NextBool(3)) // 控制粒子生成频率
                    {
                        Vector2 particlePos = position + new Vector2(
                            Main.rand.NextFloat(-50, 50),
                            Main.rand.NextFloat(-10, 10));
                        
                        Dust dust = Dust.NewDustPerfect(
                            particlePos,
                            DustID.GemSapphire, // 使用蓝宝石色粒子
                            Vector2.Zero,
                            0, dynamicColor, 1.5f);
                        
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                        
                        // 根据动态颜色调整粒子颜色
                        dust.color = dynamicColor * 0.7f;
                    }
                }
                
                spriteBatch.DrawString(FontAssets.MouseText.Value, lines[i], position, textColor);
            }

            // 在右上角绘制QQ群号
            string qqText = "Luxcinder开发交流群号:646872537";
            Vector2 qqSize = FontAssets.MouseText.Value.MeasureString(qqText);
            Vector2 qqPos = new Vector2(Main.screenWidth - qqSize.X - 20, 20);
            Color qqColor = Color.HotPink;
            
            // 绘制文本
            spriteBatch.DrawString(FontAssets.MouseText.Value, qqText, qqPos, qqColor);
            
            // 生成粉色粒子效果
            if (Main.rand.NextBool(3)) {
                Vector2 particlePos = qqPos + new Vector2(
                    Main.rand.NextFloat(0, qqSize.X * 2),  // 向右移动一整个效果图距离
                    Main.rand.NextFloat(0, qqSize.Y));
                
                int lifetime = Main.rand.Next(30, 60);
                float depth = Main.rand.NextFloat(1f, 2f);
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-1f, 1f), 
                    Main.rand.NextFloat(-1f, 1f));
                
                // 使用粉色粒子，带透明度变化
                Color particleColor = Color.Lerp(Color.HotPink, Color.White, Main.rand.NextFloat(0.3f));
                particleColor *= 0.8f;
                
                Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, particleColor, particlePos, velocity));
            }

            return false;
        }
    }



}