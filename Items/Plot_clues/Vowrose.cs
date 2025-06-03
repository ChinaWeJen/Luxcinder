using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ErosionCinder.Items.Plot_clues
{
    public class Vowrose : ModItem
    {
        public override void SetStaticDefaults()
        {

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // 提供10%全伤害抗性
            player.endurance += 0.10f;
            
            // 添加冰晶粒子效果
            if (!hideVisual && Main.rand.NextBool(10))
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.IceTorch, 0f, 0f, 150, default, 0.8f);
            }
        }

        public override void PostUpdate()
        {
            // 使玫瑰微微发光
            Lighting.AddLight(Item.Center, new Vector3(0.6f, 0.8f, 1f) * 0.6f);
        }
    }
}