using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.PlotClues
{
    public class Lostancientpendant : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
    }
}