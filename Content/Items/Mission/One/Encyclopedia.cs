using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Mission.One
{
    public class Encyclopedia : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Orange; // 红色稀有度
            Item.value = Item.buyPrice(0, 15, 0, 0);
        }


        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // 闪烁效果
            float flicker = (float)Main.rand.NextDouble() * 0.4f + 0.6f;
            Texture2D texture = Terraria.GameContent.TextureAssets.Item[Type].Value;
            Color glowColor = Color.Lerp(Color.Orange, Color.LightGoldenrodYellow, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f) * 0.6f;
            
            spriteBatch.Draw(texture, position, frame, glowColor, 0f, origin, scale, SpriteEffects.None, 0f);

            // 单个橙色光环
            float rotation = Main.GlobalTimeWrappedHourly * 1.2f;
            float radius = 12f * scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 4f;
            Vector2 offset = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * radius;
            
            spriteBatch.Draw(texture, position + offset, frame, new Color(255, 180, 60, 150) * 0.7f, 
                rotation, origin, scale * 0.45f, SpriteEffects.None, 0f);
        }

        public override void UpdateInventory(Player player)
        {
            // 背包内火焰粒子
            if (Main.rand.NextBool(12))
            {
                Vector2 position = player.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40));
                Dust dust = Dust.NewDustPerfect(position, DustID.Torch, Vector2.Zero);
                dust.noGravity = true;
                dust.scale = 1.6f;
            }
        }
    }
}