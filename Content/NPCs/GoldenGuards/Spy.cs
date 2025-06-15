using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace Luxcinder.Content.NPCs.GoldenGuards
{
    public class Spy : ModNPC
    {
        private int soundTimer = 0;
        private const int AttackCooldown = 360; // 6秒冷却(60帧/秒)
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1; // 单帧动画
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 35;
            NPC.defense = 8;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.Item62 with { Pitch = -0.2f }; // 金属撞击声
            NPC.DeathSound = SoundID.Item14 with { Volume = 0.7f }; // 小型爆炸声
            NPC.value = 750f;
            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = -1; // 自定义AI
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.buffImmune[BuffID.Confused] = true;
        }

        public override void AI()
        {
            // 状态说明:
            // ai[0] - 通用计时器
            // ai[1] - 攻击状态 (0=正常,1=上升,2=俯冲)
            // ai[2] - 攻击冷却计时器

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            // 全局左右翻转贴图锁定玩家
            NPC.spriteDirection = player.Center.X > NPC.Center.X ? -1 : 1;
            NPC.rotation = 0f; // 重置旋转角度

            // 攻击冷却计时
            if (NPC.ai[2] > 0)
            {
                NPC.ai[2]--;
            }

            // 随机触发特殊攻击(冷却结束后有1%几率触发)
            if (NPC.ai[1] == 0 && NPC.ai[2] <= 0 && Main.rand.NextBool(100))
            {
                NPC.ai[1] = 1; // 进入上升阶段
                NPC.ai[0] = 0; // 重置阶段计时器
                SoundEngine.PlaySound(SoundID.Item56 with { Pitch = 0.3f }, NPC.Center); // 机械加速声
            }

            // 状态机
            switch (NPC.ai[1])
            {
                case 0: // 正常状态 - 围绕玩家转圈
                    NormalBehavior(player);
                    break;
                    
                case 1: // 上升阶段
                    RisingPhase(player);
                    break;
                    
                case 2: // 俯冲阶段
                    DivePhase(player);
                    break;
            }

            // 通用音效逻辑
            soundTimer++;
            if (soundTimer % 40 == 0 && NPC.velocity.Length() > 0.5f && NPC.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { 
                    Pitch = -0.3f,
                    Volume = 0.6f 
                }, NPC.Center);
            }
        }

        private void NormalBehavior(Player player)
        {

                int moveDirection = NPC.velocity.X > 0 ? -1 : 1;
            // 柔和移动和转圈逻辑
            NPC.velocity.X *= 0.95f;
            NPC.velocity.Y *= 0.95f;
            
            Vector2 targetPos = player.Center + new Vector2(0, -120);
            Vector2 direction = targetPos - NPC.Center;
            float distance = direction.Length();
            
            if (distance > 100f)
            {
                direction.Normalize();
                NPC.velocity = (NPC.velocity * 10f + direction * 2f) / 11f;
            }
            else
            {
                float circleSpeed = 0.05f;
                Vector2 circleOffset = new Vector2(0, -80).RotatedBy(NPC.ai[0] * circleSpeed);
                Vector2 desiredPosition = player.Center + circleOffset;
                Vector2 desiredVelocity = (desiredPosition - NPC.Center) * 0.1f;
                
                NPC.velocity = (NPC.velocity * 15f + desiredVelocity) / 16f;
                NPC.ai[0]++;
            }
            
            if (NPC.velocity.Length() > 4f)
            {
                NPC.velocity = Vector2.Normalize(NPC.velocity) * 4f;
            }
        }

        private void RisingPhase(Player player)
        {

                int moveDirection = NPC.velocity.X > 0 ? -1 : 1;
            // 快速垂直上升
            NPC.velocity = new Vector2(0, -15f);
            NPC.ai[0]++;

            // 上升到足够高度后转为俯冲
            if (NPC.ai[0] > 60 || NPC.position.Y < player.position.Y - 800)
            {
                NPC.ai[1] = 2; // 进入俯冲阶段
                NPC.ai[0] = 0; // 重置计时器
                SoundEngine.PlaySound(SoundID.Item45 with { Volume = 0.8f }, NPC.Center); // 俯冲呼啸声
            }
        }

        private void DivePhase(Player player)
        {
            // 设置贴图方向(面向玩家)

                int moveDirection = NPC.velocity.X > 0 ? -1 : 1;

            // 随机落点范围(玩家周围200像素)
            Vector2 targetPos = player.Center + new Vector2(
                Main.rand.Next(-200, 200), 
                Main.rand.Next(100, 200));
            
            Vector2 direction = targetPos - NPC.Center;
            direction.Normalize();
            NPC.velocity = direction * 20f;
            NPC.ai[0]++;

            // 检查是否撞击地面或飞过头
            bool missedPlayer = NPC.ai[0] > 60 && Vector2.Distance(NPC.Center, player.Center) > 150;
            
            if (NPC.collideY || NPC.ai[0] > 120 || missedPlayer)
            {
                NPC.ai[1] = 0; // 返回正常状态
                NPC.ai[2] = AttackCooldown; // 设置冷却
                
                // 只在撞击地面时播放爆炸声
                if (NPC.collideY)
                {
                    SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, NPC.Center);
                    
                    // 金色粒子效果(数量减少，颜色变浅)
                    for (int i = 0; i < 6; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldFlame);
                        dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                        dust.scale = 0.9f;
                        dust.noGravity = true;
                        dust.color = new Color(255, 215, 100); // 浅金色
                    }
                }
            }
        }



        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // 返回0因为我们使用GoldenGuardsSpawnSystem控制生成
            return 0f;
        }

			// 掉落3个NonferrousMetals，100%几率
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 掉落3个NonferrousMetals，100%几率

        }
        }
    }
