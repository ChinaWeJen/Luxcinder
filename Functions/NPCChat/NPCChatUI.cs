using Luxcinder.Functions.NPCChat.Nodes;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Luxcinder.Functions.NPCChat
{
    public enum NPCChatUIState
    {
        CLOSED,
        ACTIVE
    }
    public class NPCChatUI
    {
        private int _typewriterCharCount = 0;
        private int _updateCounter = 0;
        private string _lastText = "";
        private string _currentText = "";
        private List<string> _options = new List<string>();
        private bool _isActive;
        private NPC _targetNPC;
        private int _chosenOption;

        // 打字的时候的间隔
        private int _typingInterval = 7;
        // 停顿的间隔
        private int _pauseInterval = 30;


        private Asset<Texture2D> _texture_聊天栏;
        private Asset<Texture2D> _texture_宝石;
        private Asset<Texture2D> _texture_下一步;
        private SoundStyle _sound_打字声;
        public NPCChatUI()
        {
			string relativePath = AssetExtensions.GetModRelativePathFull<NPCChatUI>();
			_texture_聊天栏 = ModContent.Request<Texture2D>($"{relativePath}/Images/聊天栏");
			_texture_宝石 = ModContent.Request<Texture2D>($"{relativePath}/Images/宝石");
			_texture_下一步 = ModContent.Request<Texture2D>($"{relativePath}/Images/下一步");
			_sound_打字声 = new SoundStyle($"{relativePath}/Sounds/对话音效")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };
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

        public void Activate(NPC npc)
        {
            if (!_isActive)
            {
                // 从休眠到唤起，清空状态
                _typewriterCharCount = 0;
                _chosenOption = -1;
                _isActive = true;
            }
            _targetNPC = npc;
        }

        public void Deactivate()
        {
            if (_isActive)
            {
                // 关闭UI窗口的瞬间发生的事情
                _isActive = false;
            }
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

            if(_options != page.Options)
            {
                _options = page.Options ?? new List<string>();
            }

            _typingInterval = page.TypewriterTypeInterval;
        }
        public string Text
        {
            get
            {
                return _currentText;
            }
        }

        public void Update()
        {
            if (_typewriterCharCount < _currentText.Length)
            {
                _updateCounter++;
                if (_updateCounter >= (_currentText[_typewriterCharCount] == ' ' ? _pauseInterval : _typingInterval))
                {
                    _typewriterCharCount++;
                    _updateCounter = 0;

                    // 播放打字机音效
                    SoundEngine.PlaySound(_sound_打字声, _targetNPC.Center);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string showText = _currentText.Substring(0, Math.Min(_typewriterCharCount, _currentText.Length));
            DrawSimpleDialogueUI(spriteBatch, showText, _targetNPC);
        }

        public void Reset()
        {
            _typewriterCharCount = 0;
            _lastText = "";
            _updateCounter = 0;
        }

        private void DrawSimpleDialogueUI(SpriteBatch spriteBatch, string text, NPC npc)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            DynamicSpriteFont value = FontAssets.MouseText.Value;
            Vector2 mainTextSize = value.MeasureString(text);
            int width = Math.Max(400, (int)mainTextSize.X + 60);
            int height = 130 + (_typewriterCharCount >= _currentText.Length ? Math.Max(0, _options.Count - 2) * 30 : 0);
            float x = (npc.Center.X - Main.screenPosition.X - width / 2);
            float y = (npc.Center.Y - Main.screenPosition.Y + height / 2);

            Texture2D panel = TextureAssets.MagicPixel.Value;
            Color bgColor = new Color(0, 0, 0, 200);
            spriteBatch.Draw(panel, new Rectangle((int)x, (int)y, width, height), bgColor);

            spriteBatch.Draw(_texture_聊天栏.Value, new Vector2(x, y + 20), Color.White);

            Color borderColor = Color.White;
            int border = 2;
            spriteBatch.Draw(panel, new Rectangle((int)(x - border), (int)(y - border), width + border * 2, border), borderColor); // 上
            spriteBatch.Draw(panel, new Rectangle((int)(x - border), (int)(y + height), width + border * 2, border), borderColor); // 下
            spriteBatch.Draw(panel, new Rectangle((int)(x - border), (int)y, border, height), borderColor); // 左
            spriteBatch.Draw(panel, new Rectangle((int)(x + width), (int)y, border, height), borderColor); // 右

            if (_typewriterCharCount < _currentText.Length)
            {
                text += "_"; // 添加光标效果
            }
            Terraria.Utils.DrawBorderString(spriteBatch, text, new Vector2(x + 40, y + 20), Color.Yellow, 1f);

            // 文本准备完毕以后才显示选项
            if (_typewriterCharCount >= _currentText.Length)
            {
                if (_options.Count == 0)
                {
                    float yDraw = y + 80;
                    float xDraw = x + width - 55;
                    spriteBatch.Draw(_texture_下一步.Value, new Vector2(xDraw, yDraw), Color.White);

                    Rectangle buttonRect = new Rectangle((int)xDraw, (int)yDraw, 45, 30);
                    PlayerInput.SetZoom_World();
                    if (buttonRect.Contains(Main.MouseScreen.ToPoint()))
                    {
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            _chosenOption = 0;
                        }
                    }
                }
                else
                {
                    foreach (var option in _options)
                    {
                        Vector2 text_size = value.MeasureString(option);
                        float yDraw = y + 60 + _options.IndexOf(option) * 30;
                        spriteBatch.Draw(_texture_宝石.Value, new Vector2(x + 10, yDraw), Color.White);

                        Rectangle buttonRect = new Rectangle((int)x + 10, (int)yDraw, (int)(text_size.X + 20), 30);
                        Color color = Color.White;
                        float scale = 1f;
                        PlayerInput.SetZoom_World();
                        if (buttonRect.Contains(Main.MouseScreen.ToPoint()))
                        {
                            color = Color.Yellow;
                            scale = 1.33f;
                            //spriteBatch.Draw(panel, new Rectangle((int)(x + 40), (int)(yDraw + text_size.Y / 2), (int)text_size.X + 10, 2), Color.White);
                            if (Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                _chosenOption = _options.IndexOf(option);
                            }
                        }
                        Terraria.Utils.DrawBorderString(spriteBatch, option, new Vector2(x + 40, yDraw + text_size.Y / 2), color, scale, 0.0f, 0.5f);
                    }
                }
            }


            PlayerInput.SetZoom_UI();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }
    }
}
