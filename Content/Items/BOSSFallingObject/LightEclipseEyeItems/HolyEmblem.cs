// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.LightEclipseEyeItems
// {
//     public class HolyEmblem : ModItem
//     {


//         public override void SetDefaults()
//         {
//             Item.width = 28;
//             Item.height = 28;
//             Item.value = Item.sellPrice(0, 3);
//             Item.rare = ItemRarityID.Pink;
//             Item.accessory = true;
//         }

//         public override void UpdateAccessory(Player player, bool hideVisual)
//         {
//             player.GetDamage(DamageClass.Magic) += 0.15f;
//             player.GetModPlayer<HolyEmblemPlayer>().holyEffect = true;
//         }

//         public override void AddRecipes()
//         {
//             Recipe recipe = CreateRecipe();
//             recipe.AddIngredient(ModContent.ItemType<CelestialFragment>(), 6);
//             recipe.AddIngredient(ItemID.SorcererEmblem);
//             recipe.AddTile(TileID.MythrilAnvil);
//             recipe.Register();
//         }
//     }

//     public class HolyEmblemPlayer : ModPlayer
//     {
//         public bool holyEffect;

//         public override void ResetEffects()
//         {
//             holyEffect = false;
//         }

//         public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
//         {
//             if (holyEffect && proj.DamageType == DamageClass.Magic && Main.rand.NextBool(5))
//             {
//                 Vector2 velocity = (target.Center - proj.Center).SafeNormalize(Vector2.Zero) * 10f;
//                 Projectile.NewProjectile(Player.GetSource_FromThis(), proj.Center, velocity, 
//                     ModContent.ProjectileType<Projectiles.SacredBeam>(), damage / 2, knockback, Player.whoAmI);
//             }
//         }
//     }
// }