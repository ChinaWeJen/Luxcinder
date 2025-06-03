using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ErosionCinder.Items.Plot_clues
{
    public class Story_fragments : ModItem
    {
        public override void SetStaticDefaults()
        {

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.maxStack = 8;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.White;
        }

        public override void PostUpdate()
        {
            // 使物品微微发光
            Lighting.AddLight(Item.Center, new Vector3(0.8f, 0.8f, 0.6f) * 0.5f);
        }

        public override void UpdateInventory(Player player)
        {
            // 更新工具提示显示当前收集进度

        }
    }
}
