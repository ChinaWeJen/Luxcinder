using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ObjectData;

namespace Luxcinder.Content.Tiles.Building
{
    public class GiantStoneBlastFurnace : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            // 7宽9高的建筑
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 7;
            TileObjectData.newTile.Height = 9;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);

            AnimationFrameHeight = 160; // 每帧高度160像素
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            // 4帧动画
            if (++frameCounter >= 6) // 每6帧切换一次
            {
                frameCounter = 0;
                frame = (frame + 1) % 4;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // 只在主线程且可见时产生粒子
            if (Main.netMode != NetmodeID.Server && Main.tile[i, j].HasTile)
            {
                // 随机产生少量粒子
                if (Main.rand.NextBool(30)) // 1/30几率产生粒子
                {
                    Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition;

                    // 烟雾粒子
                    Dust dust = Dust.NewDustPerfect(
                        position + new Vector2(Main.rand.Next(7 * 16), Main.rand.Next(9 * 16)),
                        DustID.Smoke,
                        new Vector2(0, -0.5f),
                        100, Color.Gray, 0.8f);
                    dust.noGravity = true;

                    // 火焰粒子
                    if (Main.rand.NextBool(3)) // 1/3几率产生火焰
                    {
                        Dust.NewDustPerfect(
                            position + new Vector2(Main.rand.Next(7 * 16), Main.rand.Next(9 * 16)),
                            DustID.Firefly,
                            new Vector2(0, -0.8f),
                            100, Color.Orange, 1f);
                    }
                }
            }
        }
    }
}