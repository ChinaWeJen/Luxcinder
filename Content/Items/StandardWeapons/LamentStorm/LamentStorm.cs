using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameInput;
using Luxcinder.Content.Items.StandardWeapons.LamentStorm.Projectiles;

namespace Luxcinder.Content.Items.StandardWeapons.LamentStorm
{
    public class LamentStorm : ModItem
    {
        private bool attackMode = false;
        private int windSigils = 0;
        private int chargeTime = 0;
        private bool isCharging = false;

        public int GetWindSigils() => windSigils;
        public void IncrementWindSigils() => windSigils++;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 54;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.autoReuse = true;
			Item.channel = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
            Item.damage = 45;
            Item.knockBack = 4f;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<LamentStormHoldProjectile>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void HoldItem(Player player)
        {
            // 滚轮切换攻击模式
            if (PlayerInput.ScrollWheelDelta != 0)
            {
                attackMode = !attackMode;
                Main.NewText($"切换至{(attackMode ? "从天而降" : "平射")}模式", Color.LightSkyBlue);
				player.GetModPlayer<LamentStormPlayer>().LamentStormAttackMode = attackMode ? LamentStormAttackType.Fall : LamentStormAttackType.Normal;

			}

            // 右键蓄力计时
            if (player.altFunctionUse == 2 && player.controlUseItem)
            {
                if (!isCharging)
                {
                    isCharging = true;
                    chargeTime = 0;
                }
                else
                {
                    chargeTime++;
                    if (chargeTime % 5 == 0)
                    {
                        // 蓄力粒子效果
                        var dust = Dust.NewDustPerfect(
                            player.Center + new Vector2(0, -30),
                            DustID.Electric,
                            Vector2.Zero,
                            0,
                            Color.LightSkyBlue,
                            1.5f
                        );
                        dust.noGravity = true;
                    }
                }
            }
            else if (isCharging)
            {
                isCharging = false;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				player.GetModPlayer<LamentStormPlayer>().LamentStormAttackMode = LamentStormAttackType.Charge;
			}
			else
			{
				player.GetModPlayer<LamentStormPlayer>().LamentStormAttackMode = attackMode ? LamentStormAttackType.Fall : LamentStormAttackType.Normal;
			}
			Projectile.NewProjectile(player.GetSource_ItemUse(Item, null), player.Center, Vector2.Zero, Item.shoot, damage, knockback, player.whoAmI, 0f, 0f, 0f);
			return false;
		}

		//public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		//{
		//    // 右键蓄力攻击
		//    if (player.altFunctionUse == 2)
		//    {
		//        if (chargeTime > 0)
		//        {
		//            float chargePercent = Math.Min(chargeTime / 60f, 1f); // 最大1秒蓄力
		//            int arrowCount = 5 + (int)(10 * chargePercent); // 5-15支箭
		//            int chargedDamage = (int)(damage * (1f + chargePercent)); // 伤害加成

		//            // 蓄力释放特效
		//            for (int i = 0; i < 10; i++)
		//            {
		//                var dust = Dust.NewDustPerfect(
		//                    position,
		//                    DustID.Electric,
		//                    velocity.RotatedByRandom(MathHelper.ToRadians(360)) * Main.rand.NextFloat(0.5f, 1.5f),
		//                    0,
		//                    Color.LightSkyBlue,
		//                    1.5f
		//                );
		//                dust.noGravity = true;
		//            }

		//            for (int i = 0; i < arrowCount; i++)
		//            {
		//                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
		//                perturbedSpeed *= Main.rand.NextFloat(0.8f, 1.2f) * (1f + chargePercent * 0.5f);

		//                // 创建追踪箭矢
		//                Projectile.NewProjectile(
		//                    player.GetSource_ItemUse(player.HeldItem),
		//                    position,
		//                    perturbedSpeed,
		//                    ModContent.ProjectileType<LamentStormHoming>(),
		//                    chargedDamage,
		//                    knockback * 2f,
		//                    player.whoAmI,
		//                    ai0: chargePercent >= 1f ? 1f : 0f // 完全蓄力时附加雷属性
		//                );
		//            }

		//            // 重置蓄力
		//            chargeTime = 0;
		//            isCharging = false;
		//            return false;
		//        }
		//        return false; // 未蓄力时不发射
		//    }

		//    // 左键普通攻击
		//    type = ModContent.ProjectileType<Projectiles.StandardWeaponsProjectiles.LamentStormP>();

		//    // 根据攻击模式调整发射方式
		//    if (attackMode)
		//    {
		//        // 调用从天而降的箭
		//        // 调用静态方法创建从天而降的箭
		//        Projectiles.StandardWeaponsProjectiles.LamentStormP.CreateFallingArrows(
		//            Main.MouseWorld, // 目标位置为鼠标位置
		//            damage,
		//            player.whoAmI
		//        );
		//    }
		//    else
		//    {
		//        // 平射模式 - 3-5只箭散射
		//        int arrowCount = Main.rand.Next(3, 6);
		//        for (int i = 0; i < arrowCount; i++)
		//        {
		//            Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
		//            perturbedSpeed *= Main.rand.NextFloat(0.8f, 1.2f);

		//            Projectile.NewProjectile(
		//                source,
		//                position,
		//                perturbedSpeed,
		//                type,
		//                damage,
		//                knockback,
		//                player.whoAmI
		//            );
		//        }
		//    }
		//    return false;
		//}

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 15);
            recipe.AddIngredient(ItemID.Feather, 10);
            recipe.AddIngredient(ItemID.SoulofFlight, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}