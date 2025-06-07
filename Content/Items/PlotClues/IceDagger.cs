
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Luxcinder.Content.Items.PlotClues
{
    public class IceDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 39;
            Item.knockBack = 4.5f;
            Item.crit = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.UseSound = SoundID.Item1;

            // 寒冰特效
            Item.shoot = ProjectileID.IceBolt;
            Item.shootSpeed = 8f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 20%几率冻结敌人
            if (Main.rand.NextBool(5))
            {
                target.AddBuff(BuffID.Frostburn, 180);
                // 冻结特效
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(target.position, target.width, target.height, DustID.IceTorch, 0f, 0f, 100, default, 1.5f);
                }
                SoundEngine.PlaySound(SoundID.Item28, target.position);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            // 低血量时增加攻速
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.GetAttackSpeed(DamageClass.Melee) += 0.25f;
                // 血量低时武器发光
                Lighting.AddLight(player.Center, 0.5f, 0.7f, 1f);
            }


        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // 武器挥舞时的冰晶特效
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.IceTorch);
            }
        }

        public override void AddRecipes()
        {
            // 这里可以添加合成配方
            // 示例：使用10个木材合成
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}