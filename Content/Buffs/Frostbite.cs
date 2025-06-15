using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Luxcinder.Content.Buffs;

    public class Frostbite : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // 每帧减少生命值 (60帧=1秒)
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
                
            npc.lifeRegen -= 2; // 每秒造成约30点伤害
            
            // 冰冻粒子效果
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 
                    DustID.Ice, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FrostbitePlayer>().frostbitten = true;
        }
    }

    public class FrostbitePlayer : ModPlayer
    {
        public bool frostbitten;

        public override void ResetEffects()
        {
            frostbitten = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (frostbitten)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                    
                Player.lifeRegen -= 8; // 每秒造成约20点伤害
                
                // 玩家视觉特效
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, 
                        DustID.Ice, 0f, 0f, 100, default, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }
            }
        }
    }
