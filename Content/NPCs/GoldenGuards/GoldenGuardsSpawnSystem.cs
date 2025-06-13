
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.NPCs.GoldenGuards
{
    public class GoldenGuardsSpawnSystem : ModSystem
    {
        private int spawnTimer = 0;
        private const int SpawnInterval = 300; // 5秒(60帧/秒 * 5)

        public override void PostUpdateWorld()
        {
            // 只在服务器端处理生成逻辑
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // 检查是否在森林生态
            if (!Main.player[Main.myPlayer].ZoneForest)
                return;

            // 更新计时器
            spawnTimer++;
            if (spawnTimer < SpawnInterval)
                return;

            spawnTimer = 0;

            // 随机选择一个玩家作为目标
            Player player = Main.player[Main.rand.Next(Main.player.Length)];
            if (!player.active || player.dead)
                return;

            // 在玩家附近随机位置生成NPC(确保在地面)
            int x = (int)(player.position.X + Main.rand.Next(-500, 500));
            int y = (int)(player.position.Y + Main.rand.Next(-300, 300));
            
            // 确保生成位置有效
if (!WorldGen.InWorld(x, y) || !Main.tile[x, y].HasTile)                return;

            // 检查当前GoldenGuards数量是否超过限制
            int goldenGuardsCount = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && 
                    (Main.npc[i].type == ModContent.NPCType<Guard>() || 
                     Main.npc[i].type == ModContent.NPCType<Spy>() || 
                     Main.npc[i].type == ModContent.NPCType<HolyRelicDome.HolyRelicDome>()))
                {
                    goldenGuardsCount++;
                    if (goldenGuardsCount >= 5) // 最多5个GoldenGuards
                        return;
                }
            }

            // 随机选择要生成的NPC类型
            int npcType;
            switch (Main.rand.Next(3))
            {
                case 0:
                    npcType = ModContent.NPCType<Guard>();
                    break;
                case 1:
                    npcType = ModContent.NPCType<Spy>();
                    break;
                case 2:
                    npcType = ModContent.NPCType<HolyRelicDome.HolyRelicDome>();
                    break;
                default:
                    return;
            }

            // 尝试生成NPC
            int npcIndex = NPC.NewNPC(
                Entity.GetSource_NaturalSpawn(),
                x,
                y,
                npcType
            );

            // 同步到客户端
            if (Main.netMode == NetmodeID.Server && npcIndex < Main.maxNPCs)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
            }
        }
    }
}