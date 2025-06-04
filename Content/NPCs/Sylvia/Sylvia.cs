using Luxcinder.Functions.NPCChat;
using Luxcinder.Functions.NPCChat.Flows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Content.NPCs.Sylvia
{
    public class Sylvia : ModNPC
    {
        private int currentTextIndex = 0;
        private string fullText = "";
        private string displayedText = "";
        private int textTimer = 0;
        private bool isTyping = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 20;
        }

        private NPCChatControlFlow _flow;

        public NPCChatControlFlow GetFlow
        {
            get => _flow;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 64;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.rarity = 1;
            NPC.value = Item.buyPrice(0, 5, 0, 0);

            BuildNPCChatContext();
        }

        public const string NPCInternalName = nameof(Sylvia);

        public void BuildNPCChatContext()
        {
            // 构建段落，这部分之后可以用别的工具构建，而不是写死在这里
            var para1 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q1").Value);
            var para2 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q2").Value);

            var para3_1 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A1").Value);
            var para3_2 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A2").Value);
            var para3_3 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A3").Value);

            var para3 = new NPCChatLoopBackAllOptionsParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3").Value,
                new List<(Func<string>, NPCChatParagraph)> {
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O1").Value, para3_1),
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O2").Value, para3_2),
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O3").Value, para3_3),
                });

            para1.Next = para2;
            para2.Next = para3;

            var para4 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q4").Value);
            var para5 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q5").Value);
            var para6 = new NPCChatParagraph(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q6").Value);

            para3.Next = para4;
            para4.Next = para5;
            para5.Next = para6;
            para6.Next = para6; // 让para6循环，表示结束


            // 启动流程
            _flow = new NPCChatControlFlow();
            _flow.Start(para1);
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            isTyping = true;
            textTimer = 0;
            displayedText = "";
            currentTextIndex = 0;

            List<string> dialogues = new List<string>();

            dialogues.AddRange(new[]
            {
                "你注意到了吗？这片土地正在慢慢消逝...就像被什么东西吞噬着。",
                "每当我闭上眼睛，就能听到低语...它们说着'它要来了'。",
                "我的研究显示，侵蚀现象在月圆之夜会加剧。你有观察到什么异常吗？",
                "这些天我总做同一个梦...一片灰烬之海，和一双注视着的眼睛。",
                "你相信命运吗？我觉得我们在这里相遇不是巧合。"
            });

            if (NPC.downedBoss1)
            {
                dialogues.AddRange(new[]
                {
                    "击败那个怪物后，侵蚀似乎暂停了...但只是暂时的。",
                    "我看到你战胜了那个生物。但这只是开始，更大的威胁还在后面。",
                    "我的仪器显示地下的能量读数仍然不稳定..."
                });
            }

            if (Main.hardMode)
            {
                dialogues.AddRange(new[]
                {
                    "世界正在撕裂...我能感觉到那股力量在增强。",
                    "你有没有发现，最近怪物的眼睛都泛着不自然的红光？",
                    "这不是普通的侵蚀...世界正在被重新塑造！"
                });
            }

            if (Main.bloodMoon)
            {
                dialogues.Add("血月之夜要格外小心！侵蚀速度会达到平时的5倍！");
            }

            if (Main.eclipse)
            {
                dialogues.Add("日食现象...我的研究表明这与侵蚀现象有直接关联！");
            }

            fullText = dialogues[Main.rand.Next(dialogues.Count)];
            return fullText;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "交谈";
            button2 = "研究笔记";
        }



        private string GetDialogue()
        {
            return GetChat();
        }

        private string GetResearchNotes()
        {
            var notes = new System.Text.StringBuilder();
            notes.AppendLine("这是我的研究笔记...\n\n");

            if (!NPC.downedBoss1)
            {
                notes.AppendLine("【初步观察记录】\n" +
                    "1. 侵蚀现象与月相变化相关\n" +
                    "2. 受影响区域会出现灰烬状物质\n" +
                    "3. 生物被侵蚀后会变异为攻击性形态");
            }
            else if (!Main.hardMode)
            {
                notes.AppendLine("【中期研究报告】\n" +
                    "1. 击败特定生物可暂时抑制侵蚀\n" +
                    "2. 地下深处检测到未知能量源\n" +
                    "3. 侵蚀速度与玩家进度成正比");
            }
            else
            {
                notes.AppendLine("【最终研究结论】\n" +
                    "1. 侵蚀是更高维度存在的转化过程\n" +
                    "2. 世界正在被重塑为某种容器\n" +
                    "3. 唯一阻止方法可能是...（笔记残缺）");
            }

            return notes.ToString();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.LocalPlayer.talkNPC == NPC.whoAmI)
            {
                Texture2D chatUI = ModContent.Request<Texture2D>("Luxcinder/Content/NPCs/Sylvia/DHK").Value;
                Vector2 drawPos = new Vector2(Main.screenWidth / 2 - chatUI.Width / 2, Main.screenHeight - chatUI.Height - 50);
                spriteBatch.Draw(chatUI, drawPos, Color.White);

                string npcName = NPC.GivenOrTypeName;
                Vector2 namePos = new Vector2(
                    drawPos.X + chatUI.Width / 2 - FontAssets.MouseText.Value.MeasureString(npcName).X / 2,
                    drawPos.Y + 20);
                Utils.DrawBorderString(spriteBatch, npcName, namePos, Color.White);

                if (!string.IsNullOrEmpty(Main.npcChatText))
                {
                    Vector2 textPos = new Vector2(drawPos.X + 50, drawPos.Y + 60);
                    Utils.DrawBorderString(spriteBatch, Main.npcChatText, textPos, Color.White, scale: 0.9f);
                }

                base.PostDraw(spriteBatch, screenPos, drawColor);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6.0)
            {
                NPC.frameCounter = 0.0;

                if (Math.Abs(NPC.velocity.X) > 6f)
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y < 3 * frameHeight || NPC.frame.Y >= 6 * frameHeight)
                        NPC.frame.Y = 3 * frameHeight;
                }
                else if (NPC.velocity.Y != 0)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.velocity.X != 0)
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 20 * frameHeight)
                        NPC.frame.Y = 8 * frameHeight;
                    else if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y = 8 * frameHeight;
                }
                else
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void AI()
        {
            if (NPC.velocity.X > 0)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X < 0)
                NPC.spriteDirection = 1;

            if (isTyping && currentTextIndex < fullText.Length)
            {
                textTimer++;
                if (textTimer >= 3)
                {
                    textTimer = 0;
                    displayedText += fullText[currentTextIndex++];
                    SoundEngine.PlaySound(SoundID.NPCHit1 with { Pitch = 0.8f, Volume = 0.6f });
                    Main.npcChatText = displayedText;
                }

                if (currentTextIndex >= fullText.Length)
                {
                    isTyping = false;
                }
            }
        }
    }
}