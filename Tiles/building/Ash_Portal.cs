using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Luxcinder.Tiles.building
{
    public class Ash_Portal : ModTile
    {
        public override void SetStaticDefaults()
        {
            // 基本设置
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolid[Type] = false; // 非实心
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;
            
            // 设置Tile的尺寸为22x12
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 22;
            TileObjectData.newTile.Height = 12;
            TileObjectData.newTile.CoordinateHeights = new int[12];
            for (int i = 0; i < 12; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(9, 18);
            
            // 放置条件：只需要下方有方块
            TileObjectData.newTile.AnchorBottom = new AnchorData(
                AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            
            TileObjectData.addTile(Type);

            // 设置名称和图鉴信息
            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // 破坏时掉落物品
            Item.NewItem(new EntitySource_TileBreak(i, j), 
                        i * 16, j * 16, 22 * 16, 12 * 16, 
                        ModContent.ItemType<Items.Ash_Portal>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}