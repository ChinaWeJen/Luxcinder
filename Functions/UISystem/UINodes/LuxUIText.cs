using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUIText : LuxcinderUIBase
{
    private string _text = "";
    private float _textScale = 1f;
    private Vector2 _textSize = Vector2.Zero;
    private bool _isLarge;
    private Color _color = Color.White;
    private Color _shadowColor = Color.Black;
    public bool IsInline;
    public string Text => _text.ToString();

    public float TextOriginX
    {
        get; set;
    }

    public float TextOriginY
    {
        get; set;
    }

    public Color TextColor
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;
        }
    }

    public Color ShadowColor
    {
        get
        {
            return _shadowColor;
        }
        set
        {
            _shadowColor = value;
        }
    }

    public event Action OnInternalTextChange;

    public LuxUIText(string text, float textScale = 1f, bool large = false)
    {
        TextOriginX = 0.5f;
        TextOriginY = 0f;
        InternalSetText(text, textScale, large);
    }

    public LuxUIText(LocalizedText text, float textScale = 1f, bool large = false)
    {
        TextOriginX = 0.5f;
        TextOriginY = 0f;
        InternalSetText(text.Value, textScale, large);
    }

	protected override float BindWidth(CalculatedStyle topMostDimensions)
    {
        DynamicSpriteFont dynamicSpriteFont = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, _text, Vector2.One);
        return size.X * _textScale;
    }

    protected override float BindHeight(CalculatedStyle topMostDimensions)
    {
        DynamicSpriteFont dynamicSpriteFont = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, _text, Vector2.One);
        return size.Y * _textScale;
    }

    public void SetText(string text)
    {
        InternalSetText(text, _textScale, _isLarge);
    }

    public void SetText(LocalizedText text)
    {
        InternalSetText(text.Value, _textScale, _isLarge);
    }

    public void SetText(string text, float textScale, bool large)
    {
        InternalSetText(text, textScale, large);
    }

    public void SetText(LocalizedText text, float textScale, bool large)
    {
        InternalSetText(text.Value, textScale, large);
    }

    protected override void DrawSelf(SpriteBatchX spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle dimension = GetDimensions();
        Vector2 position = dimension.Position();

        DynamicSpriteFont font = (_isLarge ? FontAssets.DeathText : FontAssets.MouseText).Value;
        Color shadowColor = _shadowColor * ((float)(int)_color.A / 255f);
        Vector2 baseScale = new Vector2(_textScale);
        TextSnippet[] snippets = ChatManager.ParseMessage(_text, _color).ToArray();
        ChatManager.ConvertNormalSnippets(snippets);
        ChatManager.DrawColorCodedStringShadow(spriteBatch.WrappedSpriteBatch, font, snippets, position, shadowColor, 0f, Vector2.Zero, baseScale, -1f, 1.5f);
        ChatManager.DrawColorCodedString(spriteBatch.WrappedSpriteBatch, font, snippets, position, Color.White, 0f, Vector2.Zero, baseScale, out var _, -1f);
    }

    private void InternalSetText(string text, float textScale, bool large)
    {
        DynamicSpriteFont dynamicSpriteFont = (large ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        _text = text;
        _isLarge = large;
        _textScale = textScale;

        if (this.OnInternalTextChange != null)
            this.OnInternalTextChange();
    }
}
