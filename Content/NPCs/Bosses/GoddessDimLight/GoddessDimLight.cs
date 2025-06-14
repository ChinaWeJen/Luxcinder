using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Luxcinder.Content.NPCs.Bosses.GoddessDimLight
{
    public class GoddessDimLight : ModNPC
    {
        // AI状态枚举
        private enum AIState
        {
            SpawnAnimation,
            Hovering,
            AttackPhase1,
            AttackPhase2,
            Despawn
        }

        // NPC属性设置
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1; // 单帧贴图
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            Music = MusicLoader.GetMusicSlot(Mod,  "Assets/Sounds/Music/ZCD/MainMenuBackgroundMusic");
            NPC.height = 80;
            NPC.lifeMax = 50000;
            NPC.defense = 40;
            NPC.damage = 70;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.aiStyle = -1; // 自定义AI
        }

        // 自定义AI逻辑
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead || !player.active)
                {
                    NPC.velocity = new Vector2(0f, -10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }

            // 根据AI状态执行不同行为
            switch ((AIState)NPC.ai[0])
            {
                case AIState.SpawnAnimation:
                    SpawnAnimation(player);
                    break;
                case AIState.Hovering:
                    HoverBehavior(player);
                    break;
                case AIState.AttackPhase1:
                    AttackPhase1(player);
                    break;
                case AIState.AttackPhase2:
                    AttackPhase2(player);
                    break;
                case AIState.Despawn:
                    DespawnBehavior();
                    break;
            }

            // 翻转贴图方向判断
            if (player.Center.X > NPC.Center.X)
            {
                NPC.spriteDirection = -1; // 反转方向
            }
            else
            {
                NPC.spriteDirection = 1; // 反转方向
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
        }

        // 震撼登场动画
        private void SpawnAnimation(Player player)
        {
            // 初始位置设置(屏幕外上方)
            if (NPC.ai[1] == 0)
            {
                NPC.position.X = player.Center.X + (Main.rand.NextBool() ? -1000 : 1000);
                NPC.position.Y = player.Center.Y - 800;
                NPC.velocity = new Vector2(player.Center.X > NPC.Center.X ? 15 : -15, 20);
                NPC.alpha = 255;
                NPC.ai[1] = 1;
                
                // 初始音效
                SoundEngine.PlaySound(SoundID.Item45 with { Pitch = -0.5f }, NPC.Center);
            }

            // 下落动画
            if (NPC.ai[1] == 1)
            {
                // 虚影效果
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50));
                    Dust dust = Dust.NewDustPerfect(pos, DustID.PurpleTorch, Vector2.Zero);
                    dust.noGravity = true;
                    dust.scale = 2f;
                    dust.alpha = 180;
                    
                    // 额外添加蓝色光效
                    if (i % 2 == 0)
                    {
                        dust = Dust.NewDustPerfect(pos, DustID.BlueTorch, Vector2.Zero);
                        dust.noGravity = true;
                        dust.scale = 2.5f;
                        dust.alpha = 150;
                    }
                }

                // 到达目标位置(玩家屏幕左右上角)
                if ((NPC.velocity.X > 0 && NPC.Center.X > player.Center.X - 300) || 
                    (NPC.velocity.X < 0 && NPC.Center.X < player.Center.X + 300))
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.ai[1] = 2;
                    NPC.ai[2] = 0;
                    
                    // 到达音效
                    SoundEngine.PlaySound(SoundID.Item72 with { Pitch = 0.3f }, NPC.Center);
                }
            }

            // 虚影合拢变实
            if (NPC.ai[1] == 2)
            {
                NPC.alpha -= 10;
                
                // 极光粒子效果
                if (NPC.ai[2] % 5 == 0)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 12) * 4f;
                        Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.PurpleTorch, vel);
                        dust.noGravity = true;
                        dust.scale = 2.5f;
                        
                        // 添加彩色粒子
                        if (i % 3 == 0)
                        {
                            dust = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, vel * 1.2f);
                            dust.noGravity = true;
                            dust.scale = 3f;
                        }
                    }
                }
                NPC.ai[2]++;

                if (NPC.alpha <= 0)
                {
                    NPC.alpha = 0;
                    
                    // 最终爆裂效果
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(8f, 15f);
                        Dust dust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool() ? DustID.PurpleTorch : DustID.BlueTorch, vel);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(2f, 4f);
                        
                        // 添加一些大型粒子
                        if (i % 5 == 0)
                        {
                            dust = Dust.NewDustPerfect(NPC.Center, DustID.Vortex, vel * 0.8f);
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(3f, 5f);
                        }
                    }
                    
                    // 震撼音效
                    SoundEngine.PlaySound(SoundID.Item100 with { Volume = 1.5f, Pitch = -0.2f }, NPC.Center);
                    
                    // 屏幕震动效果
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.LocalPlayer.AddBuff(BuffID.Obstructed, 15);
                    }
                    
                    NPC.ai[0] = (float)AIState.Hovering;
                    NPC.ai[1] = 0;
                }
            }
        }

        // 锁定悬浮行为
        private void HoverBehavior(Player player)
        {
            // 计算目标位置(屏幕左右上角)
            Vector2 targetPos = player.Center + new Vector2((NPC.Center.X < player.Center.X ? -400 : 400), -300);
            
            // 计算与目标的距离
            float distance = Vector2.Distance(NPC.Center, targetPos);
            
            // 根据距离调整移动速度
            float baseSpeed = 5f;
            float acceleration = 0.1f;
            float maxSpeed = 15f;
            
            // 距离越远速度越快
            float speedMultiplier = MathHelper.Clamp(distance / 100f, 1f, 3f);
            float currentSpeed = MathHelper.Clamp(baseSpeed + acceleration * distance, baseSpeed, maxSpeed) * speedMultiplier;
            
            // 计算移动方向
            Vector2 moveDirection = Vector2.Normalize(targetPos - NPC.Center);
            
            // 应用移动
            NPC.velocity = moveDirection * currentSpeed;
            
            // 接近目标时减速
            if (distance < 100f)
            {
                NPC.velocity *= 0.95f;
            }

            // 随机切换到攻击状态
            if (Main.rand.NextBool(120) && distance < 200f)
            {
                NPC.ai[0] = (float)AIState.AttackPhase1;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        // 第一阶段攻击
        private void AttackPhase1(Player player)
        {
            // 向玩家发射自定义弹幕
            if (NPC.ai[1] % 30 == 0)
            {
                Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center) * 10f;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, 
                    ModContent.ProjectileType<Projectiles.Bosses.GoddessDimLightAttack>(), NPC.damage / 2, 2f, NPC.whoAmI);
                
                // 添加紫蓝色粒子效果
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.PurpleTorch, 
                        velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.5f, 1f));
                    dust.noGravity = true;
                    dust.scale = 1.2f;
                }
            }

            NPC.ai[1]++;
            if (NPC.ai[1] > 180 || NPC.life < NPC.lifeMax * 0.7)
            {
                NPC.ai[0] = (float)AIState.Hovering;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        // 第二阶段攻击
        private void AttackPhase2(Player player)
        {
            // 更密集的弹幕攻击
            if (NPC.ai[1] % 15 == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(30)) * 12f;
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, 
                        ModContent.ProjectileType<Projectiles.Bosses.GoddessDimLightAttack>(), NPC.damage / 3, 1f, NPC.whoAmI);
                    
                    // 添加紫蓝色粒子效果
                    Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, 
                        velocity.RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(0.3f, 0.8f));
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }
            }

            // 新增环形弹幕攻击
            if (NPC.ai[1] % 60 == 0 && NPC.life < NPC.lifeMax * 0.4)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 12) * 6f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, 
                        ModContent.ProjectileType<Projectiles.Bosses.GoddessDimLightAttack>(), NPC.damage / 4, 1f, NPC.whoAmI);
                }
            }

            NPC.ai[1]++;
            if (NPC.ai[1] > 240 || NPC.life < NPC.lifeMax * 0.3)
            {
                NPC.ai[0] = (float)AIState.Hovering;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        // 消失行为
        private void DespawnBehavior()
        {
            NPC.alpha += 5;
            if (NPC.alpha >= 255)
            {
                NPC.active = false;
            }
        }

        // 帧动画 - 单帧版本
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0; // 始终使用第一帧
        }

        // 掉落设置
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 添加你的掉落物品
        }

        // 生成提示
        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                string text = "The dim light fades away...";
                Main.NewText(text, 175, 75, 255);
            }
        }
    }
}