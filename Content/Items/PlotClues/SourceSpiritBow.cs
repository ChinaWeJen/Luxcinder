using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace LuxCinder.Content.Items.PlotClues
{
    public class SourceSpiritBow : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 54;
            Item.scale = 1.1f;
            
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.autoReuse = true;
            
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 14;
            Item.knockBack = 2f;
            Item.crit = 4;
            
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 7.5f;
            
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // 播放射击音效
            SoundEngine.PlaySound(SoundID.Item5 with { Pitch = 0.3f }, player.Center);
            
            // 20%概率发射特殊弹幕
            if (Main.rand.NextFloat() < 0.2f)
            {
                type = ModContent.ProjectileType<SourceSpiritArrow>();
                velocity *= 1.2f; // 增加弹幕速度
            }
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            // 40%概率不消耗弹药
            return Main.rand.NextFloat() >= 0.4f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 12)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class SourceSpiritArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = 1; // 使用箭矢AI实现抛物线
            Projectile.alpha = 100; // 半透明效果
            AIType = ProjectileID.WoodenArrowFriendly;
        }

        public override void AI()
        {
            // 浅蓝色拖尾粒子
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Frost, 0f, 0f, 150, new Color(173, 216, 230, 100), 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.fadeIn = 1.2f;
            }

            // 消失时的粒子爆发
            if (Projectile.timeLeft < 10)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                        DustID.Frost, 0f, 0f, 100, new Color(200, 230, 255, 100), 1.2f);
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.noGravity = true;
                }
            }
        }

		[Obsolete]
		public override void OnKill(int timeLeft)
        {
            // 击中时的粒子效果
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Frost, 0f, 0f, 100, new Color(200, 230, 255, 100), 1.5f);
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                dust.noGravity = true;
            }
        }
    }
}