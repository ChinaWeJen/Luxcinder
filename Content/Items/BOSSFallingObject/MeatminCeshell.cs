using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.BOSSFallingObject
{
    /// <summary>
    /// 肉糜外壳 - 由腐化血肉构成的防御装备
    /// </summary>
    public class MeatminCeshell : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {


            // 添加血肉粒子效果
            if (Main.rand.NextBool(10) && !hideVisual)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 150, default, 1.5f);
            }
        }
    }
}