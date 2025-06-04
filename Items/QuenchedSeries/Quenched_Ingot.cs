
using Luxcinder.Items.BlackSeries;
using Luxcinder.Tiles.Building;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Items.QuenchedSeries
{
    public class Quenched_Ingot : ModItem
    {
        public override void SetStaticDefaults()
        {

            ItemID.Sets.SortingPriorityMaterials[Item.type] = 90;
        }

        public override void SetDefaults()
        {
             Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            
            // 使用 Swing 或 Throwing 而不是 SwingThrow
            Item.useStyle = ItemUseStyleID.Swing; // 或者 ItemUseStyleID.Throwing
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.placeStyle = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1) // 产出2个
                .AddIngredient(ModContent.ItemType<Quenched>(), 1)
                .AddIngredient(ModContent.ItemType<Black_iron_ingot>(), 2)
                .AddIngredient(ItemID.DemoniteBar, 1) // 暗影锭
                .AddTile(TileID.Hellforge) // 地狱熔炉
                .AddTile<GiantStoneBlastFurnace>()
                .Register();

            CreateRecipe(1) // 产出2个
                .AddIngredient(ModContent.ItemType<Quenched>(), 1)
                .AddIngredient(ModContent.ItemType<Black_iron_ingot>(), 2)
                .AddIngredient(ItemID.CrimtaneBar, 1) // 血腥锭
                .AddTile(TileID.Hellforge) // 地狱熔炉
                .AddTile<GiantStoneBlastFurnace>()
                .Register();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 0.8f, 0.4f, 0.1f); // 红橙色发光效果
        }
    }
}