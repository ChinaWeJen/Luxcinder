using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Luxcinder.Content.Items.BOSSFallingObject;


namespace Luxcinder.Content.Items.BOSSFallingObject
{
    public class Nightmare : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.consumable = true;
            Item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var source = player.GetSource_OpenItem(Type);

            // 必定掉落物品
            player.QuickSpawnItem(source, ModContent.ItemType<DecayedFragments>(), Main.rand.Next(1, 5));

            // 稀有掉落 (25%概率)
            if (Main.rand.NextFloat() < 0.25f)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<DecayedNightEye>());
            }
            if (Main.rand.NextFloat() < 0.25f)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<ReasonTrappedDreams>());
            }
            if (Main.rand.NextFloat() < 0.5f)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<MeatminCeshell>(), Main.rand.Next(15, 26));
            }
            // 非常稀有掉落 (10%概率)
            if (Main.rand.NextFloat() < 0.1f)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<SwordContract>());
            }

            // 专家模式额外掉落
            if (Main.expertMode)
            {
                player.QuickSpawnItem(source, ItemID.GoldCoin, Main.rand.Next(5, 10));
            }
        }
    }
}
