using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.Items.StandardWeapons.PureEdge
{
    public class PureEdge : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 100;
            Item.knockBack = 6f;
            Item.crit = 10;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<PureEdgeParticleWave>();
            Item.shootSpeed = 10f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // 武器挥舞时的粒子效果
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    new Vector2(hitbox.X, hitbox.Y),
                    hitbox.Width,
                    hitbox.Height,
                    DustID.Electric,
                    player.direction * 2,
                    0f,
                    150,
                    default(Color),
                    1.5f
                );
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // 武器在世界中的发光效果
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Vector2 position = Item.position - Main.screenPosition;
            Color glowColor = Color.Lerp(Color.Cyan, Color.White, 0.5f + (float)Main.rand.NextDouble() * 0.5f);
            
            spriteBatch.Draw(
                texture,
                position,
                null,
                glowColor,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
            
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.TerraBlade);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }

    public class PureEdgeParticleWave : ModProjectile
    {
        public override string Texture => "Luxcinder/Common/Empty"; // 使用空贴图

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255; // 完全透明
        }

        public override void AI()
        {
            // 核心粒子 - 电光效果
            for (int i = 0; i < 2; i++)
            {
                Dust electricDust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(30, 30),
                    DustID.Electric,
                    Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(1, 1),
                    100,
                    Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.8f, 1.2f)
                );
                electricDust.noGravity = true;
            }

            // 光晕粒子 - 增加视觉效果
            if (Main.rand.NextBool(3))
            {
                Dust glowDust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(20, 20),
                    DustID.Firework_Blue,
                    Projectile.velocity * 0.3f,
                    0,
                    new Color(100, 255, 255, 100),
                    Main.rand.NextFloat(1.5f, 2f)
                );
                glowDust.noGravity = true;
            }

            // 螺旋运动轨迹
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(2));
            
            // 淡出效果
            if (Projectile.timeLeft < 20)
            {
                Projectile.alpha += 5;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 不绘制任何贴图，完全依赖粒子效果
            return false;
        }
    }

    public class PureEdgeKoothrip : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            // 攻击轨迹的粒子效果
            if (Main.rand.NextBool(3))
            {
                Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51));
                Dust.NewDustPerfect(
                    dustPos,
                    DustID.Electric,
                    Vector2.Zero,
                    150,
                    Color.Cyan,
                    1f
                );
            }

            // 攻击图案动画
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 自定义绘制攻击图案
            Texture2D texture = ModContent.Request<Texture2D>("Luxcinder/Content/Items/StandardWeapons/PureEdge/PureEdgeSlash").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Color drawColor = Color.Lerp(Color.Cyan, Color.White, 0.5f);
            
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame),
                drawColor,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            
            return false;
        }
    }
}