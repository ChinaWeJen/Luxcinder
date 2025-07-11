using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.Placement
{
    public class DeepLightTower : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 27;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 30;
            Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.createTile = ModContent.TileType<Tiles.Building.DeepLightTower>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 100);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}