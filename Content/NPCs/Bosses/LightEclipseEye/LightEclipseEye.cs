using Luxcinder.Content.Projectiles.LightEclipseEye;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Luxcinder.Content.NPCs.Bosses.LightEclipseEye
{
    public class LightEclipseEye : ModNPC
    {
        // 全局移动调整常量
        private const float globalSpeedModifier = 1f;  // 降低移动速度
        private const float distanceMultiplier = 0.8f;  // 增加与玩家距离
        private const float minDistance = 200f;  // 最小跟踪距离
        private const float maxDistance = 500f;  // 最大跟踪距离


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
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss/SGZY/ZYKK");
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

        
            // 其他默认设置...

        // 其他设置...
        public override void AI()
        {
            // ===== 出场动画逻辑 =====
            if (NPC.localAI[3] == 0) // 初始状态
            {
                NPC.localAI[3] = 1; // 标记已开始
                NPC.alpha = 255; // 完全透明
                NPC.dontTakeDamage = true; // 无敌状态
                NPC.ai[3] = 0; // 重置计时器
                
                // 播放出场音效
                SoundEngine.PlaySound(new SoundStyle("Luxcinder/Assets/Music/Boss/SGZY/GDYX") with { Volume = 1.3f }, NPC.Center);
                
                // 强烈初始特效
                for (int i = 0; i < 50; i++) // 增加粒子数量
                {
                    int dustType = Main.rand.NextBool(3) ? DustID.GoldFlame : DustID.Enchanted_Gold;
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                        dustType, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), 
                        100, default, Main.rand.NextFloat(1.5f, 3f)).noGravity = true;
                }
                
                // 添加光环特效
                for (int i = 0; i < 36; i++)
                {
                    Vector2 position = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 36) * 100;
                    Dust.NewDustPerfect(position, DustID.GoldFlame, 
                        Vector2.Zero, 100, default, 2f).noGravity = true;
                }
            }

            // 渐显动画(5秒=300帧)
            if (NPC.ai[3] < 300)
            {
                NPC.ai[3]++;
                NPC.alpha = (int)(255 * (1 - NPC.ai[3]/300f)); // 线性渐显
                
                // 使BOSS旋转面朝玩家(但不影响贴图渲染)
                if (Main.player[NPC.target].active)
                {
                    Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                    NPC.rotation = direction.ToRotation() - MathHelper.PiOver2;
                    
                    // 强制重绘确保效果同步
                    NPC.spriteDirection = 1;
                    NPC.dontCountMe = true;
                }
                
                // 更丰富的持续特效
                if (Main.rand.NextBool(3)) // 增加粒子频率
                {
                    int dustType = Main.rand.NextBool() ? DustID.GoldFlame : DustID.Enchanted_Pink;
                    Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, 
                        dustType, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 
                        100, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                    
                    // 添加旋转光环粒子
                    if (NPC.ai[3] % 10 == 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 position = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * (i + NPC.ai[3]/30f)) * 80;
                            Dust.NewDustPerfect(position, DustID.GoldFlame, 
                                Vector2.Zero, 100, default, 1.8f).noGravity = true;
                        }
                    }
                }

                if (NPC.ai[3] >= 300) // 动画结束
                {
                    NPC.dontTakeDamage = false; // 取消无敌
                    // 更强烈的爆发特效
                    for (int i = 0; i < 60; i++) // 增加粒子数量
                    {
                        int dustType = i % 3 == 0 ? DustID.Enchanted_Pink : DustID.GoldFlame;
                        Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                            dustType, Main.rand.NextFloat(-6, 6), 
                            Main.rand.NextFloat(-6, 6), 100, default, 
                            Main.rand.NextFloat(1.5f, 3f)).noGravity = true;
                    }
                    
                    // 添加冲击波效果
                    for (int i = 0; i < 72; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 72) * 8;
                        Dust.NewDustPerfect(NPC.Center, DustID.GoldFlame, 
                            velocity, 100, default, 2.5f).noGravity = true;
                    }

                    // 爆炸音效
                    SoundEngine.PlaySound(new SoundStyle("Luxcinder/Assets/Sounds/SGZY/BZ") with { Volume = 1.3f }, NPC.Center);

                    // 圆形弹幕圈
                    int projectiles = 12;
                    float rotation = MathHelper.TwoPi / projectiles;
                    for (int i = 0; i < projectiles; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedBy(rotation * i) * 8f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, 
                            ModContent.ProjectileType<LightEclipseBolt>(), 40, 2f);
                    }
                }
                return; // 动画期间不执行其他AI
            }
            // ===== 结束出场动画 =====

            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
            }

            // 调整旋转使BOSS正面(贴图朝右)对准玩家
            Vector2 toPlayer = player.Center - NPC.Center;
            NPC.rotation = toPlayer.ToRotation();  // 移除了PiOver2偏移使正面朝向玩家

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
                    targetPos = player.Center + (new Vector2(0, -400) * distanceMultiplier).RotatedBy(NPC.localAI[0] * 0.015f);
                    speed = 6f * globalSpeedModifier;
                    break;
                case 2: // 突进模式
                    targetPos = player.Center + new Vector2(
                        Main.rand.Next(-300, 300) * distanceMultiplier, 
                        Main.rand.Next(-300, 300) * distanceMultiplier);
                    speed = 10f * globalSpeedModifier;
                    break;
                default: // 默认追踪模式
                    targetPos = player.Center + new Vector2(0, -350 * distanceMultiplier);
                    speed = 8f * globalSpeedModifier;
                    break;
            }

            // 确保最小距离
            Vector2 toPlayer = player.Center - NPC.Center;
            if (toPlayer.Length() < minDistance)
            {
                targetPos = player.Center + toPlayer.SafeNormalize(Vector2.Zero) * minDistance;
            }

            Vector2 direction = targetPos - NPC.Center;
            direction.Normalize();
            
            // 更平滑柔和的速度过渡
            Vector2 targetVelocity = direction * speed;
            // 添加轻微随机偏移使运动更自然
            targetVelocity += new Vector2(
                Main.rand.NextFloat(-0.3f, 0.3f),
                Main.rand.NextFloat(-0.3f, 0.3f)) * speed * 0.1f;
            // 更平缓的过渡
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.05f);

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
                case 1: // 环绕模式
                    targetPos = player.Center + (new Vector2(0, -250) * distanceMultiplier).RotatedBy(NPC.localAI[0] * 0.02f);
                    speed = 12f * globalSpeedModifier;
                    break;
                case 2: // 突袭模式
                    targetPos = player.Center + new Vector2(
                        Main.rand.Next(-200, 200) * distanceMultiplier, 
                        Main.rand.Next(-200, 200) * distanceMultiplier);
                    speed = 18f * globalSpeedModifier;
                    break;
                default: // 默认追踪模式
                    targetPos = player.Center + new Vector2(0, -300 * distanceMultiplier);
                    speed = 10f * globalSpeedModifier;
                    break;
            }

            // 确保最小距离
            Vector2 toPlayer = player.Center - NPC.Center;
            if (toPlayer.Length() < minDistance)
            {
                targetPos = player.Center + toPlayer.SafeNormalize(Vector2.Zero) * minDistance;
            }

            Vector2 direction = targetPos - NPC.Center;
            direction.Normalize();
            // 更平滑柔和的运动
            Vector2 targetVelocity = direction * speed;
            // 添加自然随机偏移
            targetVelocity += new Vector2(
                Main.rand.NextFloat(-0.4f, 0.4f),
                Main.rand.NextFloat(-0.4f, 0.4f)) * speed * 0.1f;
            // 更平缓的速度变化
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.05f);

            // 智能环形弹幕攻击
            if (NPC.ai[0]++ % 80 == 0)
            {
                int projectileCount = 6 + (int)((1f - NPC.life / NPC.lifeMax) * 4); // 血量越低越多
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / projectileCount;
                    Vector2 projectileVel = Vector2.UnitX.RotatedBy(angle) * 8f;
                    
                    // 向玩家方向偏移
                    Vector2 playerDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    projectileVel = (projectileVel + playerDir * 2f).SafeNormalize(Vector2.Zero) * 8f;
                    
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

            // 确保最小距离
            Vector2 toPlayer = player.Center - NPC.Center;
            if (toPlayer.Length() < minDistance)
            {
                NPC.velocity = toPlayer.SafeNormalize(Vector2.Zero) * -5f;
            }

            // 狂暴移动模式
            switch (NPC.localAI[1])
            {
                case 1: // 闪电冲刺模式
                    if (NPC.ai[0]++ % 60 == 0) // 增加冷却时间
                    {
                        Vector2 chargeDirection = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        NPC.velocity = chargeDirection * (22f * globalSpeedModifier); // 降低冲刺速度
                        
                        // 冲刺轨迹效果
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                                DustID.GoldFlame, 0f, 0f, 100, default, 2f);
                            dust.noGravity = true;
                        }
                    }
                    break;
                case 2: // 环绕模式
                    Vector2 orbitPos = player.Center + (new Vector2(0, -200) * distanceMultiplier).RotatedBy(NPC.localAI[0] * 0.03f); // 增加环绕距离
                    Vector2 orbitDir = (orbitPos - NPC.Center).SafeNormalize(Vector2.Zero);
                    // 更柔和的环绕运动
                    Vector2 targetOrbitVel = orbitDir * (15f * globalSpeedModifier);
                    // 添加自然随机偏移
                    targetOrbitVel += new Vector2(
                        Main.rand.NextFloat(-0.5f, 0.5f),
                        Main.rand.NextFloat(-0.5f, 0.5f)) * 2f;
                    // 更平缓的速度变化
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetOrbitVel, 0.05f);
                    break;
                default: // 默认追踪模式
                    Vector2 targetPos = player.Center + new Vector2(0, -200 * distanceMultiplier); // 增加距离
                    Vector2 direction = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, direction * (18f * globalSpeedModifier), 0.1f); // 降低速度并平滑
                    break;
            }

            // 智能弹幕风暴
            if (NPC.ai[1]++ % 30 == 0) // 降低频率
            {
                int projectileCount = 2 + (int)((1f - NPC.life / NPC.lifeMax) * 2); // 减少弹幕数量
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 spawnPos = NPC.Center + Vector2.UnitX.RotatedBy(angle) * 50f;
                    
                    // 降低精度的弹幕
                    Vector2 baseDir = (player.Center - spawnPos).SafeNormalize(Vector2.Zero);
                    Vector2 projectileVel = baseDir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 10f; // 增加散布角度并降低速度
                    
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, projectileVel,
                        ModContent.ProjectileType<LightEclipseBolt>(), 50, 2f);
                }
            }

            // 终极技能: 神圣审判(保持不变)
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