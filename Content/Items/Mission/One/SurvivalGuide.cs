using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Mission.One
{
    public class SurvivalGuide : ModItem
    {
        public override void SetStaticDefaults()
        {
            // 合成配方
            Recipe.Create(Type)
                .AddIngredient(ItemID.Acorn, 8)  // 8橡果
                .AddIngredient(ItemID.Wood, 20)  // 20木材
                .AddIngredient(ItemID.Mushroom, 5)  // 5蘑菇
                .AddTile(TileID.WorkBenches)  // 在工作台合成
                .Register();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Green; // 青色稀有度
            Item.value = Item.buyPrice(0, 10, 0, 0);
        }



        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // 脉动发光效果
            float pulse = Main.GameUpdateCount * 0.02f;
            float scaleFactor = 1f + (float)Math.Sin(pulse) * 0.1f;
            
            Texture2D texture = Terraria.GameContent.TextureAssets.Item[Type].Value;
            Color glowColor = Color.Lerp(Color.Cyan, Color.White, 0.5f + (float)Math.Sin(pulse) * 0.5f);
            
            spriteBatch.Draw(texture, position, frame, glowColor * 0.8f, 0f, origin, scale * scaleFactor, SpriteEffects.None, 0f);

            // 旋转光环
            float rotation = Main.GlobalTimeWrappedHourly * 2f;
            float radius = 12f * scale;
            Vector2 offset = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * radius;
            
            spriteBatch.Draw(texture, position + offset, frame, new Color(100, 255, 255, 150) * 0.6f, 
                rotation, origin, scale * 0.5f, SpriteEffects.None, 0f);
        }

        public override void UpdateInventory(Player player)
        {
            // 背包内粒子效果
            if (Main.rand.NextBool(15))
            {
                Vector2 position = player.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40));
                Dust dust = Dust.NewDustPerfect(position, DustID.Frost, Vector2.Zero);
                dust.noGravity = true;
                dust.scale = 1.4f;
            }
        }
    }
}