using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace Luxcinder.Content.NPCs.GoldenGuards.HolyRelicDome
{
    public class HolyRelicDome : ModNPC
    {
        private const float HoverHeight = 10f * 16; // 10格高度(像素)
        private const float WanderRange = 5f * 16; // 徘徊范围(像素)
        private int attackCooldown = 0;
        private const int AttackRate = 90; // 攻击间隔(帧)
        private int soundTimer = 0;
        private const int SoundInterval = 40; // 移动音效间隔

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5; // 5帧动画
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 0;

            // 精英怪属性

        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 400;
            NPC.damage = 15;
            NPC.defense = 20;
            NPC.knockBackResist = 0.3f;
            NPC.HitSound = SoundID.NPCHit4 with { Pitch = 0.5f };
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 1000f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1; // 自定义AI
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
            }

            // 计算目标位置(玩家头顶10格处)
            Vector2 targetPos = new Vector2(
                player.Center.X + (Main.rand.NextFloat() - 0.5f) * WanderRange * 2,
                player.Center.Y - HoverHeight + (Main.rand.NextFloat() - 0.5f) * WanderRange
            );

            // 平滑移动逻辑
            Vector2 direction = targetPos - NPC.Center;
            float distance = direction.Length();
            if (distance > 10f)
            {
                direction.Normalize();
                float speed = MathHelper.Clamp(distance / 100f, 0.5f, 5f);
                NPC.velocity = (NPC.velocity * 10f + direction * speed) / 11f;
            }
            else
            {
                NPC.velocity *= 0.95f;
            }

            // 攻击逻辑
            if (attackCooldown <= 0 && distance < 300f)
            {
                AttackPlayer(player);
                attackCooldown = AttackRate;
            }
            else if (attackCooldown > 0)
            {
                attackCooldown--;
            }

            // 移动音效
            if (soundTimer <= 0 && NPC.velocity.Length() > 1f)
            {
                SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.6f, Pitch = -0.2f }, NPC.Center);
                soundTimer = SoundInterval;
            }
            else if (soundTimer > 0)
            {
                soundTimer--;
            }

            // 旋转效果和轨迹粒子
            NPC.rotation = NPC.velocity.X * 0.05f;

            // 金色轨迹粒子
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    Main.rand.Next(new int[] { DustID.GoldFlame, DustID.GemTopaz, DustID.Enchanted_Pink }),
                    Scale: 1.2f
                );
                dust.noGravity = true;
                dust.velocity = NPC.velocity * 0.5f;
            }
        }

        private void AttackPlayer(Player player)
        {
            Vector2 direction = player.Center - NPC.Center;
            direction.Normalize();

            // 攻击音效
            SoundEngine.PlaySound(SoundID.Item75 with { Volume = 0.7f, Pitch = 0.3f }, NPC.Center);

            // 发射混合弹幕(金色、机械、神圣)
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = direction.RotatedByRandom(MathHelper.ToRadians(15)) * 6f;
                int type = Main.rand.Next(new int[] {
                    ProjectileID.GoldenShowerHostile,  // 金色弹幕
                    ProjectileID.GoldenShowerHostile,  // 金色弹幕
                    ProjectileID.GoldenShowerHostile,  // 金色弹幕
                });

                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    velocity,
                    type,
                    NPC.damage / 2,
                    2f,
                    Main.myPlayer
                );
            }

            // 混合粒子效果(金色+神圣)
            for (int i = 0; i < 15; i++)
            {
                int dustType = Main.rand.NextBool(3) ?
                    DustID.GoldFlame :
                    (Main.rand.NextBool(2) ? DustID.GemTopaz : DustID.Enchanted_Pink);

                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    dustType,
                    Scale: 1.8f
                );
                dust.noGravity = true;
                dust.velocity = direction.RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(3f, 6f);

                // 添加一些齿轮粒子表现机械感
                if (i % 3 == 0)
                {
                    Dust gearDust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.GemSapphire,
                        Scale: 1.3f
                    );
                    gearDust.noGravity = true;
                    gearDust.velocity = direction.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(1f, 3f);
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                // 死亡效果 - 金色爆炸+神圣光晕
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        i % 3 == 0 ? DustID.Enchanted_Pink : DustID.GoldFlame,
                        Scale: 2.5f
                    );
                    dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
                    dust.noGravity = true;

                    // 添加机械火花
                    if (i % 5 == 0)
                    {
                        Dust spark = Dust.NewDustDirect(
                            NPC.position,
                            NPC.width,
                            NPC.height,
                            DustID.Electric,
                            Scale: 1.8f
                        );
                        spark.velocity = Main.rand.NextVector2Circular(10f, 10f);
                        spark.noGravity = true;
                    }
                }

                // 神圣光晕效果
                for (int i = 0; i < 10; i++)
                {
                    int dustType = Main.rand.NextBool() ? DustID.GoldCoin : DustID.HallowedWeapons;
                    Dust halo = Dust.NewDustDirect(
                        NPC.Center - new Vector2(20, 20),
                        40,
                        40,
                        dustType,
                        Scale: 3f
                    );
                    halo.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);
                    halo.noGravity = true;
                }
            }
            else
            {
                // 普通受击效果 - 金色火花+机械碎片
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        Main.rand.NextBool() ? DustID.GoldFlame : DustID.GemTopaz,
                        Scale: 1.3f
                    );
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.noGravity = true;

                    // 机械碎片
                    if (i % 3 == 0)
                    {
                        Dust gear = Dust.NewDustDirect(
                            NPC.position,
                            NPC.width,
                            NPC.height,
                            DustID.GemSapphire,
                            Scale: 1f
                        );
                        gear.velocity = Main.rand.NextVector2Circular(5f, 5f);
                        gear.noGravity = true;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneOverworldHeight && Main.dayTime ? 0.05f : 0f;
        }
                public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 掉落3个NonferrousMetals，100%几率
        }
    }
}