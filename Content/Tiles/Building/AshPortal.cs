using Terraria;
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


        
    }
}