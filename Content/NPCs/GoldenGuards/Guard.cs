using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace Luxcinder.Content.NPCs.GoldenGuards
{
    public class Guard : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5; // 5帧动画
        }

        public override void SetDefaults()
        {
            NPC.width = 58;
            NPC.height = 52;
            NPC.lifeMax = 200;
            NPC.damage = 10;
            NPC.defense = 8;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.DD2_KoboldExplosion;
            NPC.value = 500f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1; // 自定义AI
        }

        public override void AI()
        {
            // 获取目标玩家
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                NPC.velocity = new Vector2(0f, -10f);
                return;
            }

            // 计算与玩家的距离和方向
            Vector2 playerPos = player.Center;
            float distanceToPlayer = NPC.Distance(playerPos);

            // 随机移动控制 - 20%几率改变移动状态
            if (Main.rand.NextBool(5))
            {
                NPC.ai[0] = Main.rand.Next(3); // 0=接近,1=远离,2=随机走动
            }

            // 平滑移动控制
            float acceleration = 0.08f; // 更缓和的加速度
            float deceleration = 0.12f; // 更快的减速
            float maxSpeed = 2.2f; // 稍微降低最大速度
            float idealDistance = 350f;
            float distanceThreshold = 150f;
            
            // 计算目标速度
            float targetSpeed = 0f;
            if (distanceToPlayer > idealDistance + distanceThreshold)
            {
                targetSpeed = maxSpeed * NPC.direction;
            }
            else if (distanceToPlayer < idealDistance - distanceThreshold)
            {
                targetSpeed = -maxSpeed * NPC.direction;
            }
            
            // 平滑速度过渡
            if (Math.Abs(NPC.velocity.X) < Math.Abs(targetSpeed))
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpeed, acceleration);
            }
            else
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetSpeed, deceleration);
            }

            // 播放移动音效
            if (Math.Abs(NPC.velocity.X) > 0.5f && NPC.ai[2]++ > 20)
            {
                SoundEngine.PlaySound(SoundID.Item35 with { Pitch = -0.5f }, NPC.Center); // 低沉机械脚步声
                NPC.ai[2] = 0;
            }

            // 随机停顿 - 15%几率进入短暂停顿
            if (Main.rand.NextBool(7) && Math.Abs(targetSpeed) < 0.1f)
            {
                NPC.velocity.X *= 0.6f; // 更明显的减速效果
                NPC.ai[0] = 20; // 缩短停顿时间
            }

            // 攻击逻辑 - 在理想距离附近时攻击
            if (distanceToPlayer < (idealDistance + distanceThreshold + 100f) && 
                distanceToPlayer > (idealDistance - distanceThreshold - 100f))
            {
                
                // 攻击逻辑
                if (NPC.ai[1]++ > 60) // 每60帧攻击一次
                {
                    Vector2 shootPos = NPC.Center;
                    Vector2 shootVel = (playerPos - shootPos).SafeNormalize(Vector2.Zero) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootPos, shootVel, 
                        ModContent.ProjectileType<Projectiles.GoldenLaser>(), 20, 3f, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item12 with { Volume = 0.7f, Pitch = -0.5f }, NPC.Center); // 低沉激光发射声
                    NPC.ai[1] = 0;
                }
            }

            // 标准化方向控制 (1=右, -1=左)
            float turnSpeed = 0.15f;
            Vector2 toPlayer = playerPos - NPC.Center;

            if (NPC.velocity.X != 0) 
            {
                // 移动方向定义(1=右, -1=左)
                int moveDirection = NPC.velocity.X > 0 ? -1 : 1;
                
                // 远离玩家时反转方向
                if ((NPC.velocity.X > 0 && toPlayer.X < 0) || 
                    (NPC.velocity.X < 0 && toPlayer.X > 0)) 
                {
                    moveDirection *= -1;
                }
                
                // 平滑转向
                if (NPC.spriteDirection != moveDirection && Main.rand.NextFloat() < turnSpeed) 
                {
                    NPC.spriteDirection = moveDirection;
                }
            }
            else 
            {
                // 静止时面向玩家
                NPC.spriteDirection = toPlayer.X > 0 ? -1 : 1;
            }

            // 待机音效 - 当NPC静止时随机播放
            if (Math.Abs(NPC.velocity.X) < 0.1f && Main.rand.NextBool(300))
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = 0.5f }, NPC.Center); // 机械运转声
            }

            // 动画控制
            const int frameCount = 5;
            const int standingFrame = 4;
            float frameHeight = NPC.height;
            
            float animationBlend = MathHelper.Clamp(Math.Abs(NPC.velocity.X) / 2.2f, 0, 1);
            
            if (animationBlend > 0.2f) {
                // 移动动画 - 确保帧索引在0-4范围内
                NPC.frameCounter += 0.1f + animationBlend * 0.2f;
                int currentFrame = (int)NPC.frameCounter % frameCount;
                if (currentFrame < 0) currentFrame += frameCount;
                
                // 动画方向同步 (与新的方向定义一致)
                if (NPC.velocity.X * NPC.spriteDirection > 0) {
                    currentFrame = frameCount - 1 - currentFrame;
                }
                
                // 精确帧定位
                NPC.frame.Y = currentFrame * (int)frameHeight;
            }
            else {
                // 直接切换到站立帧，确保精确对齐
                NPC.frame.Y = standingFrame * (int)frameHeight;
                NPC.frameCounter = standingFrame; // 重置帧计数器
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = NPC.width;
            NPC.frame.Height = frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // 返回0因为我们使用GoldenGuardsSpawnSystem控制生成
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 掉落3个NonferrousMetals，100%几率

        }
    }
}
            