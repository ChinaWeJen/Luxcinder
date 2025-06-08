using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Mission.One
{
    public class ScienceTechnologyANDInnovation : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Cyan; 
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }



        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // 星尘脉动效果
            float pulse = Main.GameUpdateCount * 0.03f;
            float scaleFactor = 1f + (float)Math.Sin(pulse) * 0.15f;
            float rotation = Main.GlobalTimeWrappedHourly * 0.5f;
            
            Texture2D texture = Terraria.GameContent.TextureAssets.Item[Type].Value;
            Color glowColor = Color.Lerp(Color.Cyan, Color.LightBlue, (float)Math.Sin(pulse * 0.7f) * 0.5f + 0.5f) * 0.6f;
            

            // 单个天蓝色光环
            float angle = Main.GlobalTimeWrappedHourly * 1.5f;
            float radius = 16f * scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 5f;
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            
            spriteBatch.Draw(texture, position + offset, frame, 
                new Color(100, 220, 255, 100) * 0.7f, 
                angle, origin, scale * 0.35f, SpriteEffects.None, 0f);
        }

        public override void UpdateInventory(Player player)
        {
            // 背包内星尘粒子
            if (Main.rand.NextBool(10))
            {
                Vector2 position = player.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40));
                Dust dust = Dust.NewDustPerfect(position, DustID.PurpleTorch, Main.rand.NextVector2Circular(1f, 1f));
                dust.noGravity = true;
                dust.scale = 1.8f;
                dust.velocity *= 0.5f;
            }
        }
    }
}