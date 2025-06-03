using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ErosionCinder.Items.Plot_clues
{
    public class Lost_ancient_pendant : ModItem
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