using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Items.BOSS_Falling_Object
{
    /// <summary>
    /// 腐化夜眼 - 被腐化的夜视饰品
    /// </summary>
    public class Decayed_Night_Eye : ModItem
    {


        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // 增益效果
            player.nightVision = true;
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 5;
            
            // 腐化副作用
            player.lifeRegen -= (int)(player.lifeRegen * 0.2f);

            // 腐化视觉粒子效果
            if (!hideVisual && Main.rand.NextBool(15))
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.5f);
            }
        }
    }
}