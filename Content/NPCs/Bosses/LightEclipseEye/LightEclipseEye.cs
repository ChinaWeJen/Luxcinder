using Luxcinder.Content.Projectiles.LightEclipseEye;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.NPCs.Bosses.LightEclipseEye
{
    public class LightEclipseEye : ModNPC
    {
        // AI状态
        private enum AttackPhase
        {
            Phase1,
            Phase2,
            Phase3
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1; // 单帧NPC
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.lifeMax = 50000;
            NPC.defense = 60;
            NPC.damage = 80;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = Item.buyPrice(0, 10);
            NPC.aiStyle = -1; // 自定义AI
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
            }

            // 面向玩家逻辑
            NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            // 神圣光环效果
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                    DustID.GoldFlame, 0f, 0f, 100, default, 2f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }

            // 根据血量切换阶段
            AttackPhase currentPhase = NPC.life > NPC.lifeMax * 0.66f ? AttackPhase.Phase1 : 
                                     NPC.life > NPC.lifeMax * 0.33f ? AttackPhase.Phase2 : 
                                     AttackPhase.Phase3;

            switch (currentPhase)
            {
                case AttackPhase.Phase1:
                    Phase1Behavior(player);
                    break;
                case AttackPhase.Phase2:
                    Phase2Behavior(player);
                    break;
                case AttackPhase.Phase3:
                    Phase3Behavior(player);
                    break;
            }
        }

        private void Phase1Behavior(Player player)
        {
            // 随机移动模式切换
            if (NPC.localAI[0]++ % 300 == 0)
            {
                NPC.localAI[1] = Main.rand.Next(3); // 0-2随机模式
            }

            Vector2 targetPos;
            float speed;
            
            switch (NPC.localAI[1])
            {
                case 1: // 环绕模式
                    targetPos = player.Center + new Vector2(0, -300).RotatedBy(NPC.localAI[0] * 0.02f);
                    speed = 8f;
                    break;
                case 2: // 快速突进模式
                    targetPos = player.Center + new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200));
                    speed = 15f;
                    break;
                default: // 默认追踪模式
                    targetPos = player.Center + new Vector2(0, -300);
                    speed = 12f;
                    break;
            }

            Vector2 direction = targetPos - NPC.Center;
            float distance = direction.Length();
            direction.Normalize();

            NPC.velocity = (NPC.velocity * 10f + direction * speed) / 11f;

            // 智能弹幕攻击
            if (NPC.ai[0]++ % 120 == 0)
            {
                int projectileCount = 3 + (int)(NPC.life / NPC.lifeMax * 2); // 根据血量增加数量
                for (int i = 0; i < projectileCount; i++)
                {
                    Vector2 offset = new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                    Vector2 projectilePos = NPC.Center + offset;
                    Vector2 predictPos = player.Center + player.velocity * (offset.Length() / 10f); // 预测玩家位置
                    Vector2 projectileVel = (predictPos - projectilePos).SafeNormalize(Vector2.Zero) * 10f;
                    
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectilePos, projectileVel, 
                        ModContent.ProjectileType<LightEclipseBolt>(), 30, 2f);
                }
            }

            // 特殊技能: 神圣光柱
            if (NPC.ai[1]++ % 600 == 0 && NPC.life < NPC.lifeMax * 0.8f)
            {
                Vector2 spawnPos = player.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-400, 400));
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, Vector2.Zero, 
                    ModContent.ProjectileType<LightEclipseBolt>(), 50, 0f);
            }
        }

        private void Phase2Behavior(Player player)
        {
            // 随机移动模式切换
            if (NPC.localAI[0]++ % 200 == 0)
            {
                NPC.localAI[1] = Main.rand.Next(3); // 0-2随机模式
            }

            Vector2 targetPos;
            float speed;
            
            switch (NPC.localAI[1])
            {
                case 1: // 快速环绕模式
                    targetPos = player.Center + new Vector2(0, -150).RotatedBy(NPC.localAI[0] * 0.03f);
                    speed = 18f;
                    break;
                case 2: // 突袭模式
                    targetPos = player.Center + new Vector2(Main.rand.Next(-150, 150), Main.rand.Next(-150, 150));
                    speed = 25f;
                    break;
                default: // 默认追踪模式
                    targetPos = player.Center + new Vector2(0, -200);
                    speed = 15f;
                    break;
            }

            Vector2 direction = targetPos - NPC.Center;
            direction.Normalize();
            NPC.velocity = (NPC.velocity * 5f + direction * speed) / 6f;

            // 智能环形弹幕攻击
            if (NPC.ai[0]++ % 80 == 0)
            {
                int projectileCount = 8 + (int)((1f - NPC.life / NPC.lifeMax) * 4); // 血量越低越多
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / projectileCount;
                    Vector2 projectileVel = Vector2.UnitX.RotatedBy(angle) * 10f;
                    
                    // 向玩家方向偏移
                    Vector2 playerDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    projectileVel = (projectileVel + playerDir * 2f).SafeNormalize(Vector2.Zero) * 10f;
                    
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVel, 
                        ModContent.ProjectileType<LightEclipseBolt>(), 40, 2f);
                }
            }


        }

        private void Phase3Behavior(Player player)
        {
            // 狂暴模式切换
            if (NPC.localAI[0]++ % 150 == 0)
            {
                NPC.localAI[1] = Main.rand.Next(3); // 0-2随机模式
            }

            // 狂暴移动模式
            switch (NPC.localAI[1])
            {
                case 1: // 闪电冲刺模式
                    if (NPC.ai[0]++ % 40 == 0)
                    {
                        Vector2 chargeDirection = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        NPC.velocity = chargeDirection * 30f;
                        
                        // 冲刺轨迹效果
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                                DustID.GoldFlame, 0f, 0f, 100, default, 2f);
                            dust.noGravity = true;
                        }
                    }
                    break;
                case 2: // 疯狂环绕模式
                    Vector2 orbitPos = player.Center + new Vector2(0, -100).RotatedBy(NPC.localAI[0] * 0.05f);
                    Vector2 orbitDir = (orbitPos - NPC.Center).SafeNormalize(Vector2.Zero);
                    NPC.velocity = (NPC.velocity * 3f + orbitDir * 20f) / 4f;
                    break;
                default: // 默认狂暴追踪模式
                    Vector2 targetPos = player.Center + new Vector2(0, -100);
                    Vector2 direction = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
                    NPC.velocity = (NPC.velocity * 2f + direction * 25f) / 3f;
                    break;
            }

            // 智能弹幕风暴
            if (NPC.ai[1]++ % 20 == 0)
            {
                int projectileCount = 2 + (int)((1f - NPC.life / NPC.lifeMax) * 4); // 血量越低越多
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 spawnPos = NPC.Center + Vector2.UnitX.RotatedBy(angle) * 50f;
                    
                    // 智能追踪弹幕
                    Vector2 predictPos = player.Center + player.velocity * 1.5f;
                    Vector2 projectileVel = (predictPos - spawnPos).SafeNormalize(Vector2.Zero) * 15f;
                    


                    
                }
            }

            // 终极技能: 神圣审判
            if (NPC.ai[2]++ % 500 == 0 && NPC.life < NPC.lifeMax * 0.3f)
            {
                // 全屏弹幕雨
                for (int i = 0; i < 24; i++)
                {
                    Vector2 spawnPos = new Vector2(
                        Main.rand.Next((int)player.position.X - 1000, (int)player.position.X + 1000),
                        player.position.Y - 800);
                    Vector2 targetVel = new Vector2(
                        Main.rand.NextFloat(-2f, 2f), 
                        Main.rand.NextFloat(10f, 15f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, targetVel, 
                        ModContent.ProjectileType<LightEclipseBolt>(), 70, 0f);
                }
                
                // 爆炸效果
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                        DustID.GoldFlame, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0; // 单帧NPC
        }

        public override void OnKill()
        {
            // 神圣爆炸效果
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                    DustID.GoldFlame, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
            }
        }
    }
}