using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ErosionCinder.Tiles.Building;

namespace ErosionCinder.Items.Placeable
{
    public class 大型建筑Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("大型建筑");
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
            Item.createTile = ModContent.TileType<大型建筑>();
        }
    }
}