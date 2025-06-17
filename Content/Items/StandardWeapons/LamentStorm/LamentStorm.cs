using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameInput;
using Luxcinder.Content.Items.StandardWeapons.LamentStorm.Projectiles;
using Terraria.Audio;

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
            Item.UseSound = SoundID.Item43; // 魔法拉弓音效
            Item.reuseDelay = 20; // 普通攻击冷却20帧
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
				// 根据蓄力时间设置冷却(20-50帧)
				Item.reuseDelay = 35 + (int)(chargeTime * 0.5f);
				chargeTime = 0; // 重置蓄力计时
			}
			else
			{
				player.GetModPlayer<LamentStormPlayer>().LamentStormAttackMode = attackMode ? LamentStormAttackType.Fall : LamentStormAttackType.Normal;
				Item.reuseDelay = 25; // 普通攻击冷却20帧
			}
			Projectile.NewProjectile(player.GetSource_ItemUse(Item, null), player.Center, Vector2.Zero, Item.shoot, damage, knockback, player.whoAmI, 0f, 0f, 0f);
			return false;
		}
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