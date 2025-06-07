using Luxcinder.Content.NPCs.Sylvia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Luxcinder.Functions.NPCChat
{
    internal class NPCChatSystem : ModSystem
    {
        private NPCChatUI _activeNPCChatUI;
        private bool _isInNPCChat;


        public override void Load()
        {
            On_Main.GUIChatDraw += On_Main_GUIChatDraw;
        }

        public override void PostSetupContent()
        {
            _activeNPCChatUI = new NPCChatUI();
        }

        private bool TryReplaceNPCChatGUI()
        {
            // 如果处于对话状态
            if ((Main.npcChatText != "" || Main.LocalPlayer.sign != -1) && !Main.editChest)
            {
                int npcID = Main.LocalPlayer.talkNPC;
                if (npcID != -1 && Main.npc[npcID].type == ModContent.NPCType<Sylvia>())
                {
                    _activeNPCChatUI.Activate(Main.npc[npcID]);
                    _activeNPCChatUI.Update();
                    _activeNPCChatUI.Draw(Main.spriteBatch);
                    return true;
                }
            }
            return false;
        }

        public override void PostUpdateNPCs()
        {
            if ((Main.npcChatText != "" || Main.LocalPlayer.sign != -1) && !Main.editChest)
            {
                int npcID = Main.LocalPlayer.talkNPC;
                if (npcID != -1 && Main.npc[npcID].type == ModContent.NPCType<Sylvia>())
                {
                    NPC npc = Main.npc[npcID];
                    if (!_isInNPCChat)
                    {
                        _isInNPCChat = true;
                        OnEnterNPCChat(npc);
                    }
                    var examplePerson = (Sylvia)npc.ModNPC;
                    // 将NPC的对话流程数据上传给UI渲染模块
                    var flow = examplePerson.GetFlow;
                    flow.Update();

                    // 读取用户交互数据
                    int option = _activeNPCChatUI.GetAndClearChosenOption();
                    if (option != -1)
                    {
                        flow.SelectOption(option);
                    }

                    _activeNPCChatUI.SetPage(flow.Current.PageInfo);
                }
            }
            else
            {
                if (_isInNPCChat)
                {
                    _isInNPCChat = false;
                }
            }
        }

        private void OnEnterNPCChat(NPC npc)
        {
            // var examplePerson = (Sylvia)npc.ModNPC;
        }

        private void On_Main_GUIChatDraw(On_Main.orig_GUIChatDraw orig, Main self)
        {
            if (!TryReplaceNPCChatGUI())
            {
                _activeNPCChatUI.Deactivate();
                orig(self);
            }
        }
    }
}
