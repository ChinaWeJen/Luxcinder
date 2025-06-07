// using Luxcinder.Content.Items.BOSSFallingObject.LightEclipseEyeItems;
// using Luxcinder.Content.Items.LightEclipseEyeItems;
// using Terraria;
// using Terraria.GameContent.ItemDropRules;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.NPCs.Bosses.LightEclipseEye
// {
//     public class LightEclipseEyeLoot : GlobalNPC
//     {
//         public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
//         {
//             if (npc.type == ModContent.NPCType<LightEclipseEye>())
//             {
//                 // 必掉物品
//                 npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 10, 15));

//                 // 概率掉落
//                 var unused = npcLoot.Add(ItemDropRule.OneFromOptions(1,
//                     ModContent.ItemType<SacredLight>(),
//                     ModContent.ItemType<HolyEmblem>(),
//                     ModContent.ItemType<CelestialFragment>()));

//                 // 专家模式专属
//                 npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<LightEclipseBag>()));
                
//                 // 10%几率掉落坐骑
//                 npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SolarWings>(), 10));
//             }
//         }

//         public override void ModifyShop(NPCShop shop)
//         {
//             if (shop.NpcType == NPCID.DD2Bartender)
//             {
//                 shop.Add<SacredLight>(Condition.DownedMechBossAny);
//             }
//         }
//     }
// }