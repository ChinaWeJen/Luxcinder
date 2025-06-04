using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.BOSSFallingObject
{
    public class Decayed_crystal : ModItem
    {


        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // 增益效果
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.GetCritChance(DamageClass.Magic) += 8;

            // 腐化副作用
            if (player.statLife > 5)
            {
                player.statLife -= 5;
                player.lifeRegenTime = 0;
            }

            // 腐化视觉效果
            if (!hideVisual)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 150, default, 1.5f);
            }
        }
    }
}