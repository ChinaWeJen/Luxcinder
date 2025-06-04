using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Luxcinder.NPCs.Bosses;
using Terraria.Audio;

namespace Luxcinder.Items.BOSS_Falling_Object
{
    public class Rotten_crystals : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 20;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            // 确保没有其他Boss存活
            return !NPC.AnyNPCs(ModContent.NPCType<CorruptedMaster>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // 生成BOSS
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CorruptedMaster>());
                
                // 视觉效果和音效
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                
                // 生成紫色粒子效果
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.PurpleTorch, 0f, 0f, 100, default, 2f);
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.RottenChunk, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}