// using Luxcinder.Content.NPCs.Bosses.LightEclipseEye;
// using Terraria.ModLoader;

// namespace Luxcinder.Content.Sounds.Music
// {
//     public class LightEclipseMusic : ModSceneEffect
//     {
//         public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/LightEclipseBattle");
//         public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
        
//         public override bool IsSceneEffectActive(Player player)
//         {
//             return NPC.AnyNPCs(ModContent.NPCType<LightEclipseEye>());
//         }
//     }
// }