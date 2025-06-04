using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;

namespace Luxcinder.Tiles.building
{
    public class Deep_Light_Tower : ModTile
    {
        public override void SetStaticDefaults()
        {
            // 基本属性设置
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            
            // 无碰撞体积设置
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            
            // 物品掉落设置

            
            // 超大建筑尺寸设置 (570x430像素)
            TileObjectData.newTile.Width = 36; // 570/16 ≈ 36
            TileObjectData.newTile.Height = 27; // 430/16 ≈ 27
            // 设置原点为中心点，使放置对准鼠标
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(
                TileObjectData.newTile.Width / 2, 
                TileObjectData.newTile.Height / 2);
            TileObjectData.newTile.CoordinateHeights = new int[TileObjectData.newTile.Height];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            
            // 初始化每个Tile的高度
            for (int i = 0; i < TileObjectData.newTile.Height; i++)
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            
            TileObjectData.addTile(Type);
            
            // 无碰撞体积
            AddMapEntry(new Microsoft.Xna.Framework.Color(200, 200, 200));
            MineResist = 1f;
            MinPick = 0;
            
            // 禁用碰撞

        }
        
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}