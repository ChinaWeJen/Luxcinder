using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.NPCChat.Nodes;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Layout;
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

	public class NPCDiaglogUI : LuxUIState
	{
		private LuxUIPanel _backgroundPanel;
		private LuxUIFramedImage _nextStepButton;
		private LuxUIText _mainTextUI;
		private LuxUIVertialAlign _optionsHanger;
		private LuxUIHorizontalSplit _nextStepBar;
		private LuxUIText _npcNameUI;
		private LuxUIImage _npcBarImage;
		private LuxUIImage _npcIconImage;

		private Asset<Texture2D> _texture_月亮;
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
		private BasicDebugDrawer _debugDrawer;

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
			Main.QueueMainThreadAction(() =>
			{
				_debugDrawer = new BasicDebugDrawer(Main.graphics.GraphicsDevice);
			});
			InitializeResources();

			_backgroundPanel = new LuxUIPanel(this.RequestModRelativeTexture("Images/TextBackground"), 32, 32, 32, 32);
			_backgroundPanel.PaddingTop = 32;
			_backgroundPanel.PaddingBottom = 32;
			_backgroundPanel.PaddingLeft = 32;
			_backgroundPanel.PaddingRight = 32;
			_backgroundPanel.MinHeight.Set(200, 0);
			_backgroundPanel.Height.SetAuto(true);

			LuxUIVertialAlign luxUIVertialAlign = new LuxUIVertialAlign();
			luxUIVertialAlign.Width.Set(0, 1f);
			luxUIVertialAlign.Height.SetAuto(true);
			_backgroundPanel.AddChild(luxUIVertialAlign);


			_nextStepButton = new LuxUIFramedImage(_texture_下一步, 1, 6);
			_nextStepButton.NormalizedOrigin = Vector2.One * 0.5f;
			_nextStepButton.OnMouseOver += _nextStepButton_OnMouseOver;
			_nextStepButton.OnMouseOut += _nextStepButton_OnMouseOut;
			_nextStepButton.OnLeftClick += _nextStepButton_OnLeftClick;


			_mainTextUI = new LuxUIText("");
			_mainTextUI.TextLayout = TextLayout.AutoWrap;
			_mainTextUI.Width.Set(0, 1);
			luxUIVertialAlign.AddChild(_mainTextUI);

			_optionsHanger = new LuxUIVertialAlign();
			_optionsHanger.Width.Set(0, 1f);
			_optionsHanger.Height.SetAuto(true);
			_optionsHanger.MarginTop = 20;
			luxUIVertialAlign.AddChild(_optionsHanger);


			var anchor = new LuxUIAnchor(_nextStepButton, Vector2.One * 0.5f, Vector2.One * 0.5f);
			_nextStepBar = new LuxUIHorizontalSplit(0.8f, null, anchor);
			_nextStepBar.Height.Set(35, 0);
			_nextStepBar.Width.Set(0, 1);

			luxUIVertialAlign.AddChild(_nextStepBar);

			_npcNameUI = new LuxUIText("");
			_npcBarImage = new LuxUIImage(_texture_月亮);
			_npcIconImage = new LuxUIImage(_texture_下一步);
			_npcIconImage.AllowResizingDimensions = false;
			_npcIconImage.Width.Set(80, 0);
			_npcIconImage.Height.Set(80, 0);
			base.AddChild(_backgroundPanel);
			base.AddChild(_npcNameUI);
			base.AddChild(_npcBarImage);
			base.AddChild(_npcIconImage);
		}

		private void _nextStepButton_OnMouseOut(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
		{
			_nextStepButton.SetImage(_texture_下一步);
			_nextStepButton.Frames = 1;
		}

	private void _nextStepButton_OnMouseOver(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
		{
			_nextStepButton.SetImage(_texture_下一步_Hover);
			_nextStepButton.Frames = 7;
		}

		private void _nextStepButton_OnLeftClick(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
        {
            if (IsReady)
            {
                _chosenOption = 0;
            }
        }

		private void InitializeResources()
        {
            string relativePath = AssetExtensions.GetModRelativePathFull<NPCChatUI>();
			_texture_月亮 = ModContent.Request<Texture2D>($"{relativePath}/Images/月亮");
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

				_npcBarImage.Top.Set(npcUIPos.Y + 5, 0f);
				_npcBarImage.Left.Set(npcUIPos.X - dimension.Width / 2 + 80, 0f);
				_npcBarImage.NormalizedOrigin = Vector2.Zero;

				_npcNameUI.SetText( targetNPC.GivenOrTypeName);
				_npcNameUI.Top.Set(npcUIPos.Y + 5, 0f); 
				_npcNameUI.Left.Set(npcUIPos.X - dimension.Width / 2 + 120, 0f);

				_npcIconImage.Top.Set(npcUIPos.Y - 50, 0f);
				_npcIconImage.Left.Set(npcUIPos.X - dimension.Width / 2, 0);
				_npcIconImage.SetImage(TextureAssets.NpcHead[NPCHeadID.Guide]);
				_npcIconImage.NormalizedOrigin = Vector2.One * 0.5f;

				bool canShowNextStep = IsReady && _options.Count == 0;
				if (canShowNextStep)
				{
					_nextStepBar.Height.Set(35, 0);
				}
				else
				{
					_nextStepBar.Height.Set(0, 0);
				}
				_nextStepButton.Visible = canShowNextStep;
				_optionsHanger.SetVisibleRecursive(IsReady);
				_nextStepButton.IgnoresMouseInteraction = !(canShowNextStep);

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

			// 更新主文本UI
			_mainTextUI.SetText(_currentText.Substring(0, Math.Min(_typewriterCharCount, _currentText.Length)));
		}

		public override void Draw(SpriteBatchX spriteBatch)
        {
			base.Draw(spriteBatch);

			//_debugDrawer.Begin(Main.UIScaleMatrix);
			//DrawDebugHitbox(_debugDrawer, true);
			//_debugDrawer.End();
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
			_optionsHanger.ClearChildren();

            if (_options.Count == 0)
			{

				return;
			}

            int index = 0;
            foreach (var option in _options)
            {
                var uiIcon = new LuxUIImage(_texture_宝石);
                var uiText = new LuxUIText(option);
				uiText.GfxOffset = new Vector2(0, 4f);
				uiText.TextLayout = TextLayout.Inline;
				int capturedIndex = index;
                uiText.OnLeftClick += (evt, ui) =>
                {
                    _chosenOption = capturedIndex;
                };
				uiText.OnMouseOver += (evt, ui) =>
				{
					uiText.TextColor = Color.Yellow;
				};
				uiText.OnMouseOut += (evt, ui) =>
				{
					uiText.TextColor = Color.White;
				};

				var alignBox = new LuxUIHorizontalSplit(0.1f, new LuxUIAnchor(uiIcon, Vector2.One * 0.5f, Vector2.One * 0.5f), 
					new LuxUIAnchor(uiText, new Vector2(0, 0.5f), new Vector2(0, 0.5f)));
				alignBox.Height.Set(30, 0);
				alignBox.Width.Set(0, 1);
				_optionsHanger.AddChild(alignBox);
                index++;
            }
        }

		private void UiText_OnMouseOver(LuxUIMouseEvent evt, LuxUIContainer listeningElement) => throw new NotImplementedException();

		public void Reset()
        {
            _typewriterCharCount = 0;
            _lastText = "";
            _updateCounter = 0;
        }
    }

    public class NPCChatUI
    {
        private LuxUI _userInterface;
        private NPCDiaglogUI _npcChatUI;

        public NPCChatUI()
        {
            _userInterface = new LuxUI();
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
