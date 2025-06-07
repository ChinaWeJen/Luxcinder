// using Luxcinder.Content.Items.LightEclipseEyeItems;
// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.BOSSFallingObject.LightEclipseEyeItems{
//     public class LightEclipseBag : ModItem
//     {
//         public override void SetDefaults()
//         {
//             Item.width = 32;
//             Item.height = 32;
//             Item.maxStack = 999;
//             Item.consumable = true;
//             Item.rare = ItemRarityID.Expert;
//             Item.expert = true;
//         }

//         public override bool CanRightClick()
//         {
//             return true;
//         }

//         public override void OpenBossBag(Player player)
//         {
//             // 专家模式专属掉落
//             player.QuickSpawnItem(ModContent.ItemType<CelestialFragment>(), Main.rand.Next(15, 25));
//             player.QuickSpawnItem(ModContent.ItemType<Accessories.HolyEmblem>());
            
//             // 25%几率额外掉落开发者物品
//             if (Main.rand.NextBool(4))
//             {
//                 player.QuickSpawnItemDirect(ItemID.DD2PetDragon);
//             }
//         }

//         public override int BossBagNPC => ModContent.NPCType<NPCs.LightEclipseEye.LightEclipseEye>();
//     }
// }