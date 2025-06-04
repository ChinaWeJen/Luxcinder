using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Luxcinder.Tiles.Building;

namespace Luxcinder.Items.Placeable
{
    public class Deep_Light_TowerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep_Light_Tower");
            Tooltip.SetDefault("一个没有碰撞体积的大型装饰性建筑");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.createTile = ModContent.TileType<Deep_Light_Tower>();
        }
    }
}