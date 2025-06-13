// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.DataStructures;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Items.LightEclipseEyeItems
// {
//     public class SacredLight : ModItem
//     {


//         public override void SetDefaults()
//         {
//             Item.width = 40;
//             Item.height = 40;
//             Item.damage = 85;
//             Item.DamageType = DamageClass.Magic;
//             Item.mana = 12;
//             Item.useTime = 25;
//             Item.useAnimation = 25;
//             Item.useStyle = ItemUseStyleID.Shoot;
//             Item.noMelee = true;
//             Item.knockBack = 4;
//             Item.value = Item.sellPrice(0, 5);
//             Item.rare = ItemRarityID.Pink;
//             Item.UseSound = SoundID.Item72;
//             Item.autoReuse = true;
//             Item.shoot = ModContent.ProjectileType<Projectiles.SacredBeam>();
//             Item.shootSpeed = 12f;
//         }

//         public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//         {
//             // 三道光束散射
//             for (int i = -1; i <= 1; i++)
//             {
//                 Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(5 * i));
//                 Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
//             }
//             return false;
//         }

//         public override void AddRecipes()
//         {
//             Recipe recipe = CreateRecipe();
//             recipe.AddIngredient(ModContent.ItemType<CelestialFragment>(), 8);
//             recipe.AddIngredient(ItemID.SoulofLight, 15);
//             recipe.AddTile(TileID.MythrilAnvil);
//             recipe.Register();
//         }
//     }
// }