using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.NPCs.GoldenGuards
{
    public class Guard : ModNPC
    {
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.GoldenGuardsItems.NonferrousMetals>(), 1, 10, 28));
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5; // 5帧动画
        }

        public override void SetDefaults()
        {
            // 基本属性
            NPC.width = 58;  // 宽度58像素
            NPC.height = 52; // 高度52像素
            NPC.lifeMax = 100; // 最大生命值
            NPC.damage = 20; // 攻击伤害
            NPC.defense = 5; // 防御力
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 100f; // 掉落金钱
            NPC.knockBackResist = 0.5f; // 击退抗性
            NPC.aiStyle = -1; // 自定义AI

            // 敌人属性
            NPC.noGravity = false; // 受重力影响
            NPC.noTileCollide = false; // 与物块碰撞
            NPC.netAlways = true;
        }

        public override void FindFrame(int frameHeight)
        {
            // 当速度接近0时也固定在第5帧
            bool isNearlyStopped = NPC.velocity.Length() < 0.5f;
            
            if (NPC.ai[2] == 1 || isNearlyStopped) // 射击状态或几乎静止
            {
                NPC.frame.Y = 4 * frameHeight; // 固定在第5帧(索引4)
                NPC.frameCounter = 0;
                
                // 射击完成后重置状态
                if (NPC.ai[1] >= 130) // 射击后10帧恢复(2秒=120帧)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[1] = 0;
                }
            }
            else
            {
                // 动态帧速 - 基于移动速度
                float speedFactor = MathHelper.Clamp(NPC.velocity.Length() / 3f, 0.5f, 2f);
                NPC.frameCounter += speedFactor;
                
                if (NPC.frameCounter >= 6) // 基础6帧调整
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = (NPC.frame.Y + frameHeight) % (5 * frameHeight); // 循环5帧
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            float distance = toPlayer.Length();

            // 智能方向控制：移动时可背对玩家，攻击时始终面向玩家
            if (NPC.ai[1] == 0f) // 移动状态
            {
                // 允许背对玩家移动 - 翻转方向
                NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;
            }
            else // 攻击/站立状态
            {
                // 确保面向玩家 - 翻转方向
                NPC.spriteDirection = player.Center.X > NPC.Center.X ? -1 : 1;
            }
            NPC.rotation = 0;

            // 保持理想距离 (300-500像素)
            float idealDistance = 400f;
            float distanceThreshold = 100f;

            if (distance > idealDistance + distanceThreshold)
            {
                // 太远 - 接近玩家 (速度提升40%)
                toPlayer.Normalize();
                NPC.velocity = Vector2.Lerp(NPC.velocity, toPlayer * 4.2f, 0.1f);
            }
            else if (distance < idealDistance - distanceThreshold)
            {
                // 太近 - 远离玩家 (速度提升40%)
                toPlayer.Normalize();
                NPC.velocity = Vector2.Lerp(NPC.velocity, -toPlayer * 2.8f, 0.1f);
            }
            else
            {
                // 理想距离 - 减速
                NPC.velocity *= 0.95f;
            }

            // 射击条件检查
            bool inShootingRange = distance > 350f && distance < 450f;
            bool isNearlyStopped = NPC.velocity.Length() < 0.5f;
            NPC.ai[1]++;
            
            if ((inShootingRange || isNearlyStopped) && NPC.ai[1] >= 120) // 2秒计时器且在射击范围内或几乎静止
            {
                float angleToPlayer = toPlayer.ToRotation();
                float facingDirection = NPC.spriteDirection == 1 ? 0 : MathHelper.Pi;
                float angleDiff = MathHelper.WrapAngle(angleToPlayer - facingDirection);

                // 只在正面90度内时射击
                if (Math.Abs(angleDiff) < MathHelper.PiOver2)
                {
                    NPC.ai[2] = 1; // 进入射击状态
                    {
                        NPC.ai[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 shootDirection = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                            int type = ModContent.ProjectileType<Projectiles.GoldenLaser>();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootDirection * 10f,
                                type, NPC.damage / 2, 3f, Main.myPlayer);
                        }
                    }
                }
            }
        }
    }
}