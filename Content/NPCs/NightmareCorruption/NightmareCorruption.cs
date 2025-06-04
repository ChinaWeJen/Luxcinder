using Luxcinder.Content.NPCs.Core.BehaviorTree;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Luxcinder.Content.NPCs.NightmareCorruption
{
    [AutoloadBossHead]
    public class NightmareCorruption : BossBase
    {
        // 快照坐标（用于类似定点攻击）
        Vector2 snapshotPos;

        // 状态变量
        private bool _isAppearing = true;
        private float _appearingProgress = 0f;
        private float _damageAccumulator = 0f; // 用于冲刺触发的伤害累计
        private bool _isSummoningMinions = false; // 是否正在召唤恍惚之握
        private bool _isChasing = false; // 是否正在追击
        private float _chaseSpeedMultiplier = 1f; // 追击速度倍率
        private int _dashCooldown = 0; // 冲刺冷却
        private int _summonCooldown = 0; // 召唤冷却
        private int _deathTimer = 0; // 死亡动画计时器

        // 幻影管理
        private List<int> _phantoms = new List<int>(); // 存储幻影NPC的whoAmI

        // 冲刺状态
        private bool _isDashing = false;
        private int _dashDuration = 0;
        private Vector2 _dashDirection = Vector2.Zero;

        // 下砸状态
        private bool _isSlamPreparing = false;
        private bool _isSlamming = false;

        public override void SetDefaults()
        {
            NPC.width = 242;
            NPC.height = 192;
            NPC.lifeMax = 13100;
            NPC.damage = 52;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;

            NPC.HitSound = new SoundStyle("Luxcinder/Assets/Sounds/Custom/NightmareCorruptionHurt");
            NPC.DeathSound = new SoundStyle("Luxcinder/Assets/Sounds/Custom/NightmareCorruptionDead");

            Main.npcFrameCount[NPC.type] = 4;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/FoulAbyssEcho");
        }

        protected override void CreateBehaviorTree()
        {
            // 使用链式构建器简化语法
            phaseBehaviors[0] = BehaviorTreeBuilder.Sequence(
                // 等待登场动画完成
                BehaviorTreeBuilder.Sequence(
                    BehaviorTreeBuilder.Condition(() => _isAppearing),
                    new ActionNode(PlayAppearingAnimation),
                    BehaviorTreeBuilder.WaitUntil(() => NPC.alpha <= 0)
                ),
                // 淡入后等待15帧稳定动画
                BehaviorTreeBuilder.WaitFrames(15),
                // 进入追击模式
                new ActionNode(() => ChaseTarget(1f))
            );

            // 阶段2行为树示例
            phaseBehaviors[1] = BehaviorTreeBuilder.Sequence(
                // 基础追击
                new ActionNode(() => ChaseTarget(1f)),
                // 每30秒召唤小怪
                BehaviorTreeBuilder.Repeat(
                    BehaviorTreeBuilder.Sequence(
                        new ActionNode(() => SummonMinions(ModContent.NPCType<GraspOfTrance>(), 3)),
                        BehaviorTreeBuilder.WaitFrames(60 * 30)
                    ),
                    -1 // 无限重复
                )
            );
        }

        public override void AI()
        {
            // 确保有目标玩家
            if (NPC.target < 0 || NPC.target >= Main.maxPlayers || !Main.player[NPC.target].active || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }

            // 首次激活检查
            if (!isActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ActivateBoss();
            }

            base.AI(); // 执行行为树

            // 更新冷却时间
            if (_dashCooldown > 0) _dashCooldown--;
            if (_summonCooldown > 0) _summonCooldown--;

            // 阶段切换：当血量低于50%时进入二阶段
            if (NPC.life <= NPC.lifeMax * 0.5f && currentPhase != 1)
            {
                ChangePhase(1);

                // 提示文本
                string message = Language.GetTextValue("Mods.Luxcinder.Content.NPCs.NightmareCorruption.Dialogue.Phase2");
                Main.NewText(message, Color.Purple);

                NPC.noGravity = false; // 二阶段受重力影响
                NPC.noTileCollide = false; // 二阶段有碰撞
            }

        }

        // ===== 具体行为实现 =====

        // 出生动画
        private NodeState PlayAppearingAnimation()
        {
            // Main.NewText($"PlayAppearingAnimation,Alpha: {NPC.alpha}");
            TargetIfRequired();
            var player = TargetPlayer;
            if (player == null)
            {
                return NodeState.Failure;
            }
            var pos = player.Center;
            pos.Y -= 270;
            NPC.Center = pos;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 4;
                return NodeState.Running;
            }
            else
            {
                NPC.alpha = 0;
                _isAppearing = false;
                PlaySound(new SoundStyle("Luxcinder/Assets/Sounds/Custom/NightmareCorruptionBorn"));
                // Main.NewText("PlayAppearingAnimationDone");
                return NodeState.Success;
            }
        }

        // 重写受伤方法以累计伤害
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            _damageAccumulator += damageDone;
            SoundEngine.PlaySound(new SoundStyle("Luxcinder/Assets/Sounds/Custom/NightmareCorruptionHurt"), NPC.Center);
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            _damageAccumulator += damageDone;
            // 播放受伤音效
            SoundEngine.PlaySound(new SoundStyle("Luxcinder/Assets/Sounds/Custom/NightmareCorruptionHurt"), NPC.Center);
        }

    }
}
