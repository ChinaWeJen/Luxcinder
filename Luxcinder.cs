using System;
using Terraria;
using Terraria.ModLoader;
namespace Luxcinder
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class Luxcinder : Mod
    {
        public override void Load()
        {
            // 注册Golden Guards死亡碎块Gore
            for (int i = 1; i <= 5; i++)
            {
                string texturePath = $"Luxcinder/Content/NPCs/GoldenGuards/NPC_Death_Fragments/{i}";
                this.AddContent(new GoldenGuardGore(texturePath, i));
            }
        }
        
        // 自定义Gore类
        public class GoldenGuardGore : ModGore
        {
            private readonly string _texturePath;
            private readonly int _index;

            public GoldenGuardGore(string texturePath, int index)
            {
                _texturePath = texturePath;
                _index = index;
            }

            public override void SetStaticDefaults()
            {
                Terraria.ID.GoreID.Sets.SpecialAI[Type] = 0;
            }

            public override string Texture => _texturePath;
            
            public override string Name => $"GoldenGuardGore_{_index}";
        }
    }
}