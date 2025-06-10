// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.LightEclipseEyeItems
// {
//     public class LightEclipseSigil : ModItem
//     {

//         public override void SetDefaults()
//         {
//             Item.width = 30;
//             Item.height = 30;
//             Item.maxStack = 20;
//             Item.value = Item.buyPrice(0, 5);
//             Item.rare = ItemRarityID.Pink;
//             Item.useAnimation = 30;
//             Item.useTime = 30;
//             Item.useStyle = ItemUseStyleID.HoldUp;
//             Item.consumable = true;
//         }

//         public override bool CanUseItem(Player player)
//         {
//             return !NPC.AnyNPCs(ModContent.NPCType<NPCs.LightEclipseEye.LightEclipseEye>());
//         }

//         public override bool? UseItem(Player player)
//         {
//             if (player.whoAmI == Main.myPlayer)
//             {
//                 Vector2 spawnPos = player.Center + new Vector2(0, -500);
//                 NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.LightEclipseEye.LightEclipseEye>());
                
//                 // 神圣召唤效果
//                 for (int i = 0; i < 30; i++)
//                 {
//                     Dust dust = Dust.NewDustDirect(spawnPos, 10, 10, 
//                         DustID.GoldFlame, 0f, 0f, 100, default, 3f);
//                     dust.noGravity = true;
//                     dust.velocity *= 3f;
//                 }
//             }
//             return true;
//         }

//         public override void AddRecipes()
//         {
//             Recipe recipe = CreateRecipe();
//             recipe.AddIngredient(ItemID.SoulofLight, 10);
//             recipe.AddIngredient(ItemID.SoulofNight, 10);
//             recipe.AddIngredient(ItemID.GoldBar, 5);
//             recipe.AddTile(TileID.MythrilAnvil);
//             recipe.Register();
//         }
//     }
// }