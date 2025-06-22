using System;
using System.Collections.Generic;
using Humanizer;
using Luxcinder.Content.Items.Mission.One;
using Luxcinder.Functions.MissionSystem;
using Luxcinder.Functions.MissionSystem.Core;
using Luxcinder.Functions.MissionSystem.UI;
using Luxcinder.Functions.NPCChat;
using Luxcinder.Functions.NPCChat.Nodes;
using Luxcinder.Functions.UISystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Main.npcFrameCount[Type] = 19;
        }

        private NPCChatControlFlow _flow;

        public NPCChatControlFlow GetFlow
        {
            get => _flow;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 70;
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
            var para1 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q1").Value);
            var para2 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q2").Value);

            var para3_1 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A1").Value);
            var para3_2 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A2").Value);
            var para3_3 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_A3").Value);

            var para3 = new ExhaustiveOptionsNode(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3").Value,
                new List<(Func<string>, NPCChatNode)> {
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O1").Value, para3_1),
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O2").Value, para3_2),
                    (() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q3_O3").Value, para3_3),
                });

            para1.Next = para2;
            para2.Next = para3;

            var para4 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q4").Value);
            var para5 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q5").Value);
            var para6 = new PlainTextCondition(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q6T").Value, 
				() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q6").Value,
				() => Main.LocalPlayer.GetModPlayer<MissionPlayer>().CheckIfMissionCompleted("Sylvia_Books"));

			var para7 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q7").Value);
			var para8 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q8").Value);
			var para9 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q9").Value);
			var para10 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q10").Value);
			var para11 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q11").Value);
			var para12 = new PlainText(() => Mod.GetLocalization($"{NPCInternalName}.Dialogue.FirstMet.Q12").Value);

			para3.Next = para4;
			para4.Next = para5;
			para5.Next = para6;
			para6.Next = para7;
			para7.Next = para8;
			para8.Next = para9;
			para9.Next = para10;
			para10.Next = para11;
			para11.Next = para12;
			para12.Next = null; // 结束对话


			// 启动流程
			_flow = new NPCChatControlFlow();
            _flow.Start(para1);

			para4.OnUserChooseOption += Para4_OnUserChooseOption;
			para6.OnUserChooseOption += Para6_OnUserChooseOption;
		}

		private void Para6_OnUserChooseOption(int index)
		{
			if (index != 0)
				return;
			var modPlayer = Main.LocalPlayer.GetModPlayer<MissionPlayer>();
			if (modPlayer.CheckIfMissionCompleted("Sylvia_Books"))
			{
				modPlayer.CompleteMission("Sylvia_Books");
			}
		}

		private void Para4_OnUserChooseOption(int index)
		{
			if (index != 0)
				return;

			if (Main.LocalPlayer.talkNPC == NPC.whoAmI)
			{
				var mission = new Mission
				{
					Id = "Sylvia_Books",
					Name = Mod.GetLocalization($"{NPCInternalName}.Missions.First_Books.Title"),
					Description = Mod.GetLocalization($"{NPCInternalName}.Missions.First_Books.Description"),
					Status = MissionStatus.NotAccepted,
					Conditions = new List<MissionCondition>
					{
						new ItemCollectCondition(ModContent.ItemType<Encyclopedia>(), 1),
						new ItemCollectCondition(ModContent.ItemType<ScienceTechnologyANDInnovation>(), 1),
						new ItemCollectCondition(ModContent.ItemType<SurvivalGuide>(), 1)
					}
				};
				// 设置接受任务动作
				Main.LocalPlayer.GetModPlayer<MissionPlayer>().AcceptMission(mission);
				LuxUISystem.SetActive<MissionPageUI_InventoryLayer>(true);
				LuxUISystem.GetUI<MissionPageUI_InventoryLayer>().SetMission(mission);
			}
			
		}

		public override bool CanChat()
        {
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.LocalPlayer.talkNPC == NPC.whoAmI)
            {
                Texture2D chatUI = ModContent.Request<Texture2D>("Luxcinder/Content/NPCs/Sylvia/DHK").Value;
                Vector2 drawPos = new Vector2(
                    Main.screenWidth / 2 - chatUI.Width / 2 - chatUI.Width,  // 向左移动1个贴图宽度
                    Main.screenHeight - chatUI.Height - 50 - chatUI.Height * 11);  // 向上移动11个贴图高度
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

                // 投掷动作检测(帧1-4)
                if (NPC.ai[1] == 1)  // 假设使用ai[1]作为投掷标志
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y < 1 * frameHeight || NPC.frame.Y >= 5 * frameHeight)
                        NPC.frame.Y = 1 * frameHeight;
                }
                // 掉落状态(帧5)
                else if (NPC.velocity.Y != 0)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                // 行走动画(帧6-18)
                else if (NPC.velocity.X != 0)
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 19 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    else if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                }
                // 站立(帧0)
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