using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Luxcinder.Content.Tiles.Building
{
    public class AshPortal : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            // 设置建筑尺寸 (22x12 tiles)
            TileObjectData.newTile.Width = 22;
            TileObjectData.newTile.Height = 12;
            TileObjectData.newTile.CoordinateHeights = new int[12];
            for (int i = 0; i < 12; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(11, 11); // 中心点

            // 允许玩家穿过建筑
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileBlockLight[Type] = false;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Microsoft.Xna.Framework.Color(150, 150, 150), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition;

                // 绘制静态传送门中心贴图
                if (ModContent.HasAsset("Luxcinder/Assets/Textures/Tiles/PortalEffect"))
                {
                    Texture2D portalEffect = ModContent.Request<Texture2D>("Luxcinder/Assets/Textures/Tiles/PortalEffect").Value;
                    // 以TileObjectData原点(11,11)为基础，向右下偏移四倍距离
                    Vector2 baseCenter = position + new Vector2(11 * 16, 11 * 16);
                    Vector2 portalCenter = baseCenter + new Vector2(12 * 16, 7 * 16 + 8); // X偏移12tile，Y偏移7tile-8像素
                    // 4帧垂直动画参数
                    int frameCount = 4;
                    int frameHeight = portalEffect.Height / frameCount;
                    int frame = (int)(Main.GameUpdateCount / 9 % frameCount); // 每5帧切换一次
                    Rectangle frameRect = new Rectangle(0, frame * frameHeight, portalEffect.Width, frameHeight);

                    spriteBatch.Draw(
                        portalEffect,
                        portalCenter,
                        frameRect, // 使用动画帧区域
                        Color.White * 0.9f,
                        0f, // 不旋转
                        new Vector2(portalEffect.Width / 2f, frameHeight / 2f), // 调整原点为中心
                        1f, // 大小
                        SpriteEffects.None,
                        0f);

                    // 添加发光扩散效果
                    float lightIntensity = 1.2f;
                    int radius = 8; // 8 tiles半径
                    
                    // 中心强光
                    Lighting.AddLight(portalCenter, 
                        0.7f * lightIntensity, // R
                        0.5f * lightIntensity, // G 
                        1.0f * lightIntensity); // B (紫色光)
                    
                    // 向外扩散的渐变光
                    for (int x = -radius; x <= radius; x++)
                    {
                        for (int y = -radius; y <= radius; y++)
                        {
                            float distance = (float)Math.Sqrt(x * x + y * y);
                            if (distance <= radius)
                            {
                                float decay = 1f - (distance / radius);
                                Vector2 lightPos = portalCenter + new Vector2(x * 16, y * 16);
                                Lighting.AddLight(lightPos,
                                    0.5f * decay * lightIntensity,
                                    0.3f * decay * lightIntensity,
                                    0.8f * decay * lightIntensity);
                            }
                        }
                    }
                }
            }
        }
    }
}

    