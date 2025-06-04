using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Luxcinder.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Luxcinder.Tiles.building;

namespace Luxcinder.Items.Quenched_Series
{
    public class Quenched : ModItem
    {

                // 世界中的掉落物粒子效果（数量减少40%）
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Item == null || Item.IsAir)
                return;

            // 柔和光源效果
            Lighting.AddLight(Item.position, new Vector3(0.6f, 0.3f, 0.1f));

            // 25%几率生成世界中的火焰粒子（原2个现1个）
            if (Main.rand.NextBool(4))
            {
                Dust fire = Dust.NewDustDirect(
                    Item.position,
                    Item.width,
                    Item.height,
                    DustID.Torch,
                    Main.rand.NextFloat(-0.5f, 0.5f),
                    Main.rand.NextFloat(-0.5f, 0.5f),
                    150, // 半透明
                    new Color(255, 150, 50, 150), // 橙黄色
                    1.2f
                );
                if (fire != null)
                {
                    fire.noGravity = true;
                    fire.fadeIn = 0.8f;
                    fire.velocity *= 0.6f;
                }
            }
        }


        public override void SetDefaults()
        {
            // 物品基本设置
            Item.width = 16;    // 物品宽度（像素）
            Item.height = 16;   // 物品高度（像素）
            Item.maxStack = 999; // 最大堆叠数量

            Item.useTurn = true; // 使用时会转向
            Item.autoReuse = true; // 自动重复使用
            Item.useAnimation = 15; // 使用动画时间
            Item.useTime = 10;     // 使用间隔时间
            Item.useStyle = ItemUseStyleID.Swing; // 使用动作
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.consumable = true; // 物品会被消耗

            // 设置放置的Tile类型
            Item.createTile = ModContent.TileType<Quenched_Tile>();

            // 物品稀有度
            Item.rare = ItemRarityID.Orange;
// ItemRarityID.White`（白色）：适合基础材料，如矿石、木材等。
// ItemRarityID.Blue/Green`（蓝/绿）：适合早期游戏装备或普通魔法物品。
// ItemRarityID.Orange/LightRed`（橙/浅红）：适合Boss掉落或困难模式初期装备。
// ItemRarityID.Pink/LightPurple`（粉/浅紫）：适合后期高级装备。
// ItemRarityID.Yellow/Red`（黄/红）：适合终极装备或大师模式专属物品。

            // 物品价值（铜币）
      
        }

        // 可选：添加合成配方
        public override void AddRecipes()
        {
            CreateRecipe(10)

		        .AddTile<Giant_Stone_Blast_Furnace_Tiles>()
                .Register();
        }
    }
}