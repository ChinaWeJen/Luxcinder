using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder
{
    public class RecipeSystem : ModSystem
    {
        public override void AddRecipeGroups()
        {
            // 铜锭/锡锭配方组
            int[] copperOrTin = new int[] { ItemID.CopperBar, ItemID.TinBar };
            RecipeGroup group = new RecipeGroup(() => "铜锭或锡锭", copperOrTin);
            RecipeGroup.RegisterGroup("Luxcinder:CopperOrTinBars", group);
        }

        public override void AddRecipes()
        {
            // Encyclopedia配方
            Recipe.Create(ModContent.ItemType<Content.Items.Mission.One.Encyclopedia>())
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient(ItemID.DirtBlock, 30)
                .AddIngredient(ItemID.WorkBench, 1)
                .AddTile(TileID.WorkBenches)
                .Register();

            // ScienceTechnologyANDInnovation配方
            Recipe.Create(ModContent.ItemType<Content.Items.Mission.One.ScienceTechnologyANDInnovation>())
                .AddIngredient(ItemID.IronBar, 5)
                .AddRecipeGroup("Luxcinder:CopperOrTinBars", 5)
                .AddIngredient(ItemID.Gel, 20)
                .AddTile(TileID.WorkBenches)
                .Register();

            // SurvivalGuide配方
            Recipe.Create(ModContent.ItemType<Content.Items.Mission.One.SurvivalGuide>())
                .AddIngredient(ItemID.Acorn, 8)
                .AddIngredient(ItemID.Wood, 20)
                .AddIngredient(ItemID.Mushroom, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}