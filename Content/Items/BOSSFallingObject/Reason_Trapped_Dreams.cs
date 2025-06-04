using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Luxcinder.Content.Projectiles;

namespace Luxcinder.Content.Items.BOSSFallingObject
{
    public class Reason_Trapped_Dreams : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 12);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item72;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CorruptedOrb>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 在玩家头顶上方发射3个弹幕
            for (int i = 0; i < 3; i++)
            {
                // 计算散射角度 (40度间隔)
                float angleOffset = MathHelper.ToRadians(-40 + i * 40);
                Vector2 spawnPosition = player.Center + new Vector2(0, -70).RotatedBy(angleOffset * 0.6f);

                // 计算速度方向
                Vector2 shootVelocity = velocity.RotatedBy(angleOffset);
                shootVelocity.Normalize();
                shootVelocity *= Item.shootSpeed;

                // 发射弹幕
                Projectile.NewProjectile(
                    source,
                    spawnPosition,
                    shootVelocity,
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}