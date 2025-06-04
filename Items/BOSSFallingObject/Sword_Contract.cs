﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System.Collections.Generic;

namespace Luxcinder.Items.BOSSFallingObject
{
    public class Sword_Contract : ModItem
    {
        private bool buffActive = false;
        private int originalMaxHP = 0;
        


        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(gold: 15);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (buffActive)
            {
                tooltips.Add(new TooltipLine(Mod, "Sacrificed", "[已祭献]") {
                    OverrideColor = Color.Red
                });
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (!buffActive)
                {
                    // 保存原始最大生命值
                    originalMaxHP = player.statLifeMax2;
                    
                    // 永久减少30%最大生命值
                    int newMaxHP = (int)(player.statLifeMax * 0.7f);
                    player.statLifeMax -= newMaxHP;
                    
                    // 调整当前生命值，确保不超过新上限
                    if (player.statLife > newMaxHP)
                        player.statLife = newMaxHP;
                    
                    CombatText.NewText(player.getRect(), Color.Red, $"MAX HP -{originalMaxHP - newMaxHP}");
                    
                    buffActive = true;
                    Item.damage = (int)(Item.damage * 1.5f);
                    
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustDirect(
                            Position: player.position,
                            Width: player.width,
                            Height: player.height,
                            Type: DustID.LifeDrain,
                            SpeedX: Main.rand.NextFloat(-3f, 3f),
                            SpeedY: Main.rand.NextFloat(-3f, 3f),
                            Alpha: 100,
                            newColor: default,
                            Scale: 1.5f
                        );
                    }
                    
                    SoundEngine.PlaySound(SoundID.Item104, player.position);
                    Main.NewText("契约生效! 永久牺牲30%最大生命值,伤害提升50%", Color.Purple);
                }
                else
                {
                    // 恢复原始最大生命值
                    player.statLifeMax2 = originalMaxHP;
                    buffActive = false;
                    Item.damage = (int)(Item.damage / 1.5f);
                    
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustDirect(
                            Position: player.position,
                            Width: player.width,
                            Height: player.height,
                            Type: DustID.PurpleTorch,
                            SpeedX: Main.rand.NextFloat(-3f, 3f),
                            SpeedY: Main.rand.NextFloat(-3f, 3f),
                            Alpha: 100,
                            newColor: default,
                            Scale: 1.5f
                        );
                    }
                    
                    SoundEngine.PlaySound(SoundID.Item105, player.position);
                    Main.NewText("契约解除，恢复最大生命值", Color.Purple);
                }
                
                return false;
            }
            
            return base.CanUseItem(player);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(
                    Position: target.position,
                    Width: target.width,
                    Height: target.height,
                    Type: DustID.Shadowflame,
                    SpeedX: Main.rand.NextFloat(-2f, 2f),
                    SpeedY: Main.rand.NextFloat(-2f, 2f),
                    Alpha: 100,
                    newColor: default,
                    Scale: 1.2f
                );
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustDirect(
                    Position: new Vector2(hitbox.X, hitbox.Y),
                    Width: hitbox.Width,
                    Height: hitbox.Height,
                    Type: DustID.PurpleTorch,
                    SpeedX: Main.rand.NextFloat(-3f, 3f),
                    SpeedY: Main.rand.NextFloat(-3f, 3f),
                    Alpha: 150,
                    newColor: default,
                    Scale: 1.3f
                );
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.DarkShard, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}