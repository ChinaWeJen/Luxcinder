using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.BOSSFallingObject
{
    /// <summary>
    /// 腐化碎片 - 来自腐化之地的黑暗材料
    /// </summary>
    public class DecayedFragments : ModItem
    {


        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            // 添加微弱的紫色发光效果
            Lighting.AddLight(Item.Center, 0.5f, 0f, 0.8f);
        }
    }
}