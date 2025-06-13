using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Luxcinder.Content.Items.GoldenGuardsItems
{
    public class NonferrousMetals : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
        }
    }
}