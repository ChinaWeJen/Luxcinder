using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace Luxcinder.Content.Items.StandardWeapons.SeaAttack
{
    public class SeaAttack : ModItem
    {
        public override void SetStaticDefaults()
        {


            
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            // 基础属性
            Item.width = 60;
            Item.height = 60;
            Item.scale = 1.2f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3, 50, 0); // 3金50银
            
            // 使用属性
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;
            Item.useTurn = true;
            
            // 武器属性
            Item.DamageType = DamageClass.Melee;
            Item.damage = 58;
            Item.knockBack = 5.5f;
            Item.crit = 7;
            Item.shoot = ModContent.ProjectileType<Projectiles.SeaAttack.SeaWave>();
            Item.shootSpeed = 14f;
            
            // 视觉效果
            Item.UseSound = SoundID.Item1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 生成主弹幕
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            
            // 水花粒子效果
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(position, Item.width, Item.height, 
                            DustID.Water, velocity.X * 0.4f, velocity.Y * 0.4f, 
                            150, default, 1.4f);
            }
            
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // 挥动时的水波效果
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 
                                      DustID.Water, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddIngredient(ItemID.Coral, 15)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            // 武器在世界中的发光效果
            Texture2D texture = ModContent.Request<Texture2D>("Luxcinder/Content/Items/StandardWeapons/SeaAttack/SeaAttack_Glow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f),
                           null, Color.White * 0.8f, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}