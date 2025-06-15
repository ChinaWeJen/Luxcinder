
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

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
            Item.noUseGraphic = true; // 隐藏默认武器贴图
            Item.noMelee = true; // 禁用默认近战攻击

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

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            // 加载自定义动画贴图（带错误处理）
            Texture2D texture;
            try {
                texture = ModContent.Request<Texture2D>("Luxcinder/Content/Dynamic/WeaponsDynamic/IceDaggerDynamic").Value;
                if (texture == null || texture.IsDisposed) {
texture = ModContent.Request<Texture2D>(Texture).Value; // 使用当前物品的自定义贴图                    Main.NewText("IceDagger动画贴图加载失败，使用默认贴图", Color.Orange);
                }
            }
            catch {
texture = ModContent.Request<Texture2D>(Texture).Value; // 使用当前物品的自定义贴图                Main.NewText("IceDagger动画贴图加载失败，使用默认贴图", Color.Orange);
            }
            
            // 调试信息：显示贴图加载状态
if (texture == TextureAssets.Item[Item.type].Value)
             {
                Main.NewText("正在使用备用贴图", Color.Yellow);
            } else {
                Main.NewText($"已加载动画贴图，尺寸: {texture.Width}x{texture.Height}", Color.Lime);
            }

            // 14帧动画参数 (每帧128x128像素)
            const int frameCount = 14;
            const int frameWidth = 128;
            const int frameHeight = 128;
            
            // 验证贴图尺寸
            if(texture.Width != frameWidth || texture.Height < frameCount * frameHeight) {
                Main.NewText($"贴图尺寸应为{frameWidth}x{frameCount*frameHeight} (当前:{texture.Width}x{texture.Height})", Color.Red);
                texture = TextureAssets.Item[Item.type].Value; // 强制使用备用贴图
            }

            // 计算当前动画帧 (每5游戏帧切换1动画帧)
            int frame = (int)(Main.GameUpdateCount / 5) % frameCount;
            
            // 调试信息：显示当前帧
            if(Main.GameUpdateCount % 30 == 0) {
                Main.NewText($"动画帧: {frame+1}/{frameCount}", Color.Cyan);
            }
            
            // 计算绘制区域和原点
            Rectangle frameRect = new Rectangle(0, frame * frameHeight, frameWidth, frameHeight);
            Vector2 origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter) - Main.screenPosition;
            
            // 计算鼠标方向旋转
            float rotation = (player.Center - Main.MouseWorld).ToRotation();
            if (player.direction == -1) {
                rotation += MathHelper.Pi;
            }
            
            // 开始精灵批处理
            var spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.GameViewMatrix.TransformationMatrix);

            // 绘制14帧动画
            spriteBatch.Draw(
                texture,
                position,
                frameRect,
                Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16),
                rotation,
                origin,
                Item.scale,
                player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f);

            spriteBatch.End();
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