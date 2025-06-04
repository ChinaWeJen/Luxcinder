using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Luxcinder.Items
{
    public class Ash_Portal : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            // 物品基础设置
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            
            // 使用设置
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;

            
            // 关联Tile
            Item.createTile = ModContent.TileType<Tiles.building.Ash_Portal>();
            
            // 稀有度
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            // 基础合成配方
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Glass, 50);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}