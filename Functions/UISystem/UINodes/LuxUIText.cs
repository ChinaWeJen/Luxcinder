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
using static System.Net.Mime.MediaTypeNames;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUIText : LuxUIContainer
{
    private string _text = "";
    private string _visibleText = "";
    private float _textScale = 1f;
	private Vector2 _gfxOffset;
    private bool _isLarge;
    private Color _color = Color.White;
    private Color _shadowColor = Color.Black;
    public TextLayout TextLayout
    {
        get;
        set;
    }
    public string Text
    {
        get
        {
            return _text;
        }
    }

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

	public Vector2 GfxOffset
	{
		get => _gfxOffset;
		set => _gfxOffset = value;
	}


	public event Action OnInternalTextChange;

    public LuxUIText(string text, float textScale = 1f, bool large = false)
    {
        TextLayout = TextLayout.Inline;
        TextOriginX = 0.5f;
        TextOriginY = 0f;
        InternalSetText(text, textScale, large);
    }

    public LuxUIText(LocalizedText text, float textScale = 1f, bool large = false) : this(text.Value, textScale, large)
    {

    }

    public override void InitializeDependencies()
    {
        
        base.InitializeDependencies();
    }

    public override void ResolveDependencies() 
    {
        DynamicSpriteFont dynamicSpriteFont = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        if (TextLayout == TextLayout.AutoWrap)
        {
            _visibleText = dynamicSpriteFont.CreateWrappedText(_text, _width.TypedValue / _textScale);
        }
        else
        {
            _visibleText = _text;
        }
        base.ResolveDependencies(); 
    }

    protected override float ResolveWidth(CalculatedStyle topMostDimensions)
    {
        switch (TextLayout)
        {
            case TextLayout.Inline:
                {
                    DynamicSpriteFont dynamicSpriteFont = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
                    Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, _text, Vector2.One);
                    return size.X * _textScale;
                }
            case TextLayout.Block:
                return base.ResolveWidth(topMostDimensions);
            case TextLayout.AutoWrap:
                return base.ResolveWidth(topMostDimensions);
            default:
                return base.ResolveWidth(topMostDimensions);
        }
    }

    protected override float ResolveHeight(CalculatedStyle topMostDimensions)
    {
        DynamicSpriteFont dynamicSpriteFont = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        switch (TextLayout)
        {
            case TextLayout.Inline:
                {
                    Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, _visibleText, Vector2.One);
                    return size.Y * _textScale;
                }
            case TextLayout.Block:
                return base.ResolveHeight(topMostDimensions);
            case TextLayout.AutoWrap:
                {
                    Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, _visibleText, Vector2.One);
                    return size.Y * _textScale;
                }
            default:
                return base.ResolveHeight(topMostDimensions);
        }
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
        CalculatedStyle dimension = GetDimensions();
        Vector2 position = dimension.Position() + _gfxOffset;

        DynamicSpriteFont font = (_isLarge ? FontAssets.DeathText : FontAssets.MouseText).Value;
        Color shadowColor = new Color(0, 0, 0, _color.A);
        Vector2 baseScale = new Vector2(_textScale);
        TextSnippet[] snippets = ChatManager.ParseMessage(_visibleText, _color).ToArray();

        spriteBatch.Push(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        ChatManager.ConvertNormalSnippets(snippets);
        ChatManager.DrawColorCodedStringShadow(spriteBatch.WrappedSpriteBatch, font, snippets, position, shadowColor, 0f, Vector2.Zero, baseScale, -1f, 1.5f);
        ChatManager.DrawColorCodedString(spriteBatch.WrappedSpriteBatch, font, snippets, position, _color, 0f, Vector2.Zero, baseScale, out var _, -1f);
        spriteBatch.Pop();
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
