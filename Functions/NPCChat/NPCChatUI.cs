using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.NPCChat.Nodes;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.Utils;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Luxcinder.Functions.NPCChat
{
    public enum NPCChatUIState
    {
        CLOSED,
        ACTIVE
    }

    public class NPCDiaglogUI : UIState
    {
        private LuxcinderUIPanel _backgroundPanel;
        private LuxcinderUIImageButton _nextStepButton;
        private UIList _listOptions;

        private Asset<Texture2D> _texture_聊天栏;
        private Asset<Texture2D> _texture_宝石;
        private Asset<Texture2D> _texture_下一步;
        private Asset<Texture2D> _texture_下一步_Hover;
        private SoundStyle _sound_打字声;

        private int _typewriterCharCount = 0;
        private int _updateCounter = 0;
        private string _lastText = "";
        private string _currentText = "";
        private List<string> _options = new List<string>();
        private bool _isActive;
        private NPC _targetNPC;
        private int _chosenOption;

        private int _typingInterval;
        private List<string> _warppedText = new List<string>();


        public NPC TargetNPC
        {
            get => _targetNPC;
            set => _targetNPC = value;
        }
        public bool IsReady
        {
            get
            {
                return _typewriterCharCount >= _currentText.Length;
            }
        }

        public int GetAndClearChosenOption()
        {
            int result = _chosenOption;
            _chosenOption = -1;
            return result;
        }

        public override void OnInitialize()
        {
            InitializeResources();

            var textureBackground = this.RequestModRelativeTexture("Images/TextBackground");
            _backgroundPanel = new LuxcinderUIPanel(textureBackground, 32, 32, 32, 32);
            _backgroundPanel.BackgroundColor = Color.White;
            _backgroundPanel.BorderColor = Color.White;

            _nextStepButton = new LuxcinderUIImageButton(_texture_下一步);
            _nextStepButton.SetHoverImage(_texture_下一步_Hover);
            _nextStepButton.Width.Set(68, 0);
            _nextStepButton.Height.Set(22, 0);
			_nextStepButton.OnLeftClick += _nextStepButton_OnLeftClick;
            _nextStepButton.SetVisibility(1.0f, 0.5f);

            _listOptions = new UIList();

            _backgroundPanel.Append(_listOptions);
            _backgroundPanel.Append(_nextStepButton);

            base.Append(_backgroundPanel);
        }

        private void _nextStepButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (IsReady)
            {
                _chosenOption = 0;
            }
        }

		private void InitializeResources()
        {
            string relativePath = AssetExtensions.GetModRelativePathFull<NPCChatUI>();
            _texture_聊天栏 = ModContent.Request<Texture2D>($"{relativePath}/Images/聊天栏");
            _texture_宝石 = ModContent.Request<Texture2D>($"{relativePath}/Images/宝石");
            _texture_下一步 = ModContent.Request<Texture2D>($"{relativePath}/Images/NextStep");
            _texture_下一步_Hover = ModContent.Request<Texture2D>($"{relativePath}/Images/NextStep_Hover");
            _sound_打字声 = new SoundStyle($"{relativePath}/Sounds/对话音效")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
        }

        public override void OnActivate()
        {
            if (!_isActive)
            {
                // 从休眠到唤起，清空状态
                _typewriterCharCount = 0;
                _chosenOption = -1;
                _isActive = true;
            }
        }

        public override void OnDeactivate()
        {
            _isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // 如果NPC对话框处于活动状态，更新UI位置
            if (Main.LocalPlayer.talkNPC != -1)
            {
                NPC targetNPC = Main.npc[Main.LocalPlayer.talkNPC];
                _backgroundPanel.Width.Set(500, 0);
                int textHeight = 30 * _warppedText.Count + Math.Max(1, _options.Count) * 30 + 30;
                _backgroundPanel.Height.Set(Math.Max(200, textHeight), 0);

                var dimension = _backgroundPanel.GetDimensions();

                // 将NPC在Game View中的位置转换到UI坐标下
                Vector2 npcScreenPos = targetNPC.Center - Main.screenPosition;
                Vector2 npcUIPos = Vector2.Transform(npcScreenPos, Main.GameViewMatrix.ZoomMatrix);
                // 现在 npcUIPos 就是UI坐标，可以用于UI元素定位
                npcUIPos = Vector2.Transform(npcUIPos, Matrix.Invert(Main.UIScaleMatrix));

                _backgroundPanel.Left.Set(npcUIPos.X - dimension.Width / 2, 0f);
                _backgroundPanel.Top.Set(npcUIPos.Y + 46, 0f);


                _nextStepButton.Top.Set(-30, 1);
                _nextStepButton.Left.Set(-60, 1);
                _nextStepButton.IsActive = IsReady && _options.Count == 0;

                if (_backgroundPanel.IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }

            // 打字机效果更新
            if (_typewriterCharCount < _currentText.Length)
            {
                _updateCounter++;
                if (_updateCounter >= _typingInterval)
                {
                    _typewriterCharCount += 2;
                    _typewriterCharCount = Math.Min(_typewriterCharCount, _currentText.Length);
                    _updateCounter = 0;

                    // 播放打字机音效
                    SoundEngine.PlaySound(_sound_打字声, _targetNPC.Center);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            {

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Main.UIScaleMatrix);

                base.Draw(spriteBatch);

                spriteBatch.End();
            }

            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                float x = (_backgroundPanel.Left.Pixels);
                float y = (_backgroundPanel.Top.Pixels);

                string showText = _currentText.Substring(0, Math.Min(_typewriterCharCount, _currentText.Length));
                var innerDimension = _backgroundPanel.GetInnerDimensions();
                _warppedText = UIUtils.WrapText(showText, FontAssets.MouseText.Value, innerDimension.Width);

                int lineNum = 0;
                foreach (var line in _warppedText)
                {
                    Terraria.Utils.DrawBorderString(spriteBatch, line, new Vector2(innerDimension.X , innerDimension.Y  + 30 * lineNum), Color.White, 1f);
                    lineNum++;
                }
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }


        public void SetPage(NPCChatPage page)
        {
            _currentText = page.Text;
            if (_currentText != _lastText)
            {
                if (page.ImmediateShow)
                {
                    _typewriterCharCount = _currentText.Length; // 立即显示全部文本
                }
                else
                {
                    _typewriterCharCount = 0; // 重置打字机计数
                }
                _updateCounter = 0; // 重置更新时间
                _chosenOption = -1;
                _lastText = _currentText; // 更新最后的文本

            }

            // 将此处改为列表里的所有元素的字符串相等

            if (!(_options?.SequenceEqual(page.Options ?? new List<string>()) ?? (page.Options == null)))
            {
                _options = page.Options ?? new List<string>();
                ResetOptionsUI();
            }

            _typingInterval = page.TypewriterTypeInterval;
        }

        private void ResetOptionsUI()
        {
            _listOptions.Clear();

            if (_options.Count == 0)
                return;

            int index = 0;
            foreach (var option in _options)
            {
                var alignBox = new LuxcinderUIHorizontalAlign();
                var uiIcon = new UIImage(_texture_宝石);
                uiIcon.Width.Set(16, 0);
                uiIcon.Height.Set(16, 0);
                uiIcon.MarginRight = 16f;
                var uiText = new LuxcinderUIText(option);
                uiText.TextScale = 1f;
				uiText.OnMouseOver += UiText_OnMouseOver;
				uiText.OnMouseOut += UiText_OnMouseOut;

                int capturedIndex = index;
                uiText.OnLeftClick += (evt, ui) =>
                {
                    _chosenOption = capturedIndex;
                };

                alignBox.Append(uiIcon);
                alignBox.Append(uiText);
                _listOptions.Add(alignBox);
                index++;
            }
            int textHeight = 30 * _warppedText.Count;

            var bgDimension = _backgroundPanel.GetInnerDimensions();
            _listOptions.Width.Set(bgDimension.Width, 0);
            _listOptions.Height.Set(100, 0);
            _listOptions.Top.Set(textHeight + 5, 0);
        }

		private void UiText_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            LuxcinderUIText uiText = (LuxcinderUIText)listeningElement;
            uiText.TextScale = 1f;
            uiText.TextColor = Color.White;
        }

		private void UiText_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            LuxcinderUIText uiText = (LuxcinderUIText)listeningElement;
            uiText.TextColor = Color.Yellow;
        }


		public void Reset()
        {
            _typewriterCharCount = 0;
            _lastText = "";
            _updateCounter = 0;
        }
    }

    public class NPCChatUI
    {
        private UserInterface _userInterface;
        private NPCDiaglogUI _npcChatUI;

        public NPCChatUI()
        {
            _userInterface = new UserInterface();
            _npcChatUI = new NPCDiaglogUI();
            _npcChatUI.Initialize();
        }



        public void Activate(NPC npc)
        {
            _npcChatUI.TargetNPC = npc;
            _userInterface.SetState(_npcChatUI);
        }

        public int GetAndClearChosenOption()
        {
            return _npcChatUI.GetAndClearChosenOption();
        }


        public void Update(GameTime gameTime)
        {
            _userInterface.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _userInterface.Draw(spriteBatch, Main._drawInterfaceGameTime);
        }

        public void SetPage(NPCChatPage pageInfo)
        {
            _npcChatUI.SetPage(pageInfo);
        }
        public void Deactivate()
        {
            _userInterface.SetState(null);
            _npcChatUI.Deactivate();
        }
	}
}
