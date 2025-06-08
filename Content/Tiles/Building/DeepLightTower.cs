using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Luxcinder.Content.Tiles.Building
{
    public class DeepLightTower : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            
            // 设置建筑尺寸 (570x430像素)
            TileObjectData.newTile.Width = 36; // 570/16 ≈ 36
            TileObjectData.newTile.Height = 27; // 430/16 ≈ 27
            TileObjectData.newTile.CoordinateHeights = new int[27];
            for (int i = 0; i < 27; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(18, 22); // 大幅调整Y值解决悬空问题
            
            // 允许玩家穿过建筑
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileBlockLight[Type] = false;
            
            TileObjectData.addTile(Type);
            
LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // 掉落物品逻辑
        }
    }
}