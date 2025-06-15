global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ID;
using Terraria.Audio;
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

            // 安全加载音乐资源
            try 
            {
MusicLoader.GetMusicSlot(this, "Assets/Music/Boss/SGZY/ZYKK.ogg");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load music: {ex.Message}");
                // 可以在这里添加备用音乐或空处理
            }

            // 菜单系统已通过Autoload自动加载
            Logger.Info("Menu systems initialized");

            // 使用Autoload方式自动加载ModMenu
            // 确保LuxcinderMenu.cs类有[Autoload]属性

            // 暂时注释掉自定义主菜单设置
            // ModLoader.TryGetMod("Luxcinder", out Mod luxcinderMod);
            // Main.MenuUI.SetState(new UI.LuxcinderMainMenu(luxcinderMod));
            // Logger.Info("LuxcinderMenu initialized");
        }

        public override void Unload()
        {
            try
            {
                // 防御性释放资源
                // tModLoader会自动处理大部分资源释放
                // 这里主要确保自定义资源的清理
                
                // 示例：清理静态字段或单例
                // if (MyStaticResource != null)
                // {
                //     MyStaticResource.Dispose();
                //     MyStaticResource = null;
                // }

                base.Unload();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during mod unloading: {ex}");
                throw; // 重新抛出以确保tModLoader知道卸载失败
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