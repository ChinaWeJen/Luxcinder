using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LuxCinder.Content.Effects
{
    public class MenuHaloEffect : ModSystem
    {
        private float intensity;
        private float pulseSpeed;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["LuxCinder:MenuHalo"] = new Filter(
                    new ScreenShaderData("FilterMiniTower")
                        .UseColor(1f, 0.9f, 0.8f)
                        .UseOpacity(0), 
                    EffectPriority.VeryHigh);
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.menuMode == 0) // 仅在主菜单显示
            {
                // 使用主音量作为强度基础
                intensity = Main.musicVolume * 0.8f;
                pulseSpeed = 0.5f + intensity * 0.3f;
                
                // 更新着色器参数
                if (Filters.Scene["LuxCinder:MenuHalo"]?.IsActive() == true)
                {
                    Filters.Scene["LuxCinder:MenuHalo"].GetShader()
                        .UseIntensity(intensity)
                        .UseProgress(pulseSpeed);
                }
            }
            else
            {
                intensity = 0;
            }
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.menuMode == 0 && intensity > 0.1f)
            {
                Filters.Scene["LuxCinder:MenuHalo"].Activate(Main.screenPosition, Main.ScreenSize);
                Filters.Scene["LuxCinder:MenuHalo"].GetShader()
                    .UseColor(
                        MathHelper.Lerp(0.8f, 1.2f, intensity),
                        MathHelper.Lerp(0.7f, 1.1f, intensity),
                        MathHelper.Lerp(0.6f, 1.0f, intensity))
                    .UseOpacity(intensity * 0.3f);
                
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                
                // 绘制全屏光晕
                Texture2D tex = ModContent.Request<Texture2D>("Terraria/Images/Misc/noise").Value;
                Rectangle rect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(tex, rect, Color.White * intensity * 0.5f);
                
                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
    }
}