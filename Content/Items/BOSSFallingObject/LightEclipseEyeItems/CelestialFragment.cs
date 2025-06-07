// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.LightEclipseEyeItems
// {
//     public class CelestialFragment : ModItem
//     {
//         public override void SetStaticDefaults()
//         {

//             ItemID.Sets.ItemNoGravity[Type] = true;
//         }

//         public override void SetDefaults()
//         {
//             Item.width = 24;
//             Item.height = 24;
//             Item.maxStack = 999;
//             Item.value = Item.sellPrice(0, 0, 25);
//             Item.rare = ItemRarityID.Pink;
//         }

//         public override void PostUpdate()
//         {
//             Lighting.AddLight(Item.Center, 0.5f, 0.4f, 0.1f);
//         }
//     }
// }