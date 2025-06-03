using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ErosionCinder.Items.BOSS
{
    /// <summary>
    /// 腐化毒素 - 由腐化之王体内提取的剧毒物质
    /// </summary>
    public class Rotting_toxin : ModItem
    {


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