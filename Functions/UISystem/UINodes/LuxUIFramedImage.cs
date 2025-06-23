using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUIFramedImage : LuxUIContainer
{
    private Asset<Texture2D> _texture;
    public float ImageScale
    {
        get;
        set;
    }
    public Color Color = Color.White;
    public Vector2 NormalizedOrigin
    {
        get;
        set;
    }

    public int Frames
    {
        get => _frames;
        set
        {
            if (value > 0)
            {
                _frames = value;
            }
        }
    }

	public int FrameTime
	{
		get => _frameTime;
		set
		{
			if (value > 0)
			{
				_frameTime = value;
			}
		}
	}

    private int _frames;
    private int _frameTime;
    private int _currentFrame = 0;
    private int _frameCounter = 0;

    public LuxUIFramedImage(Asset<Texture2D> texture, int frames, int frameTime)
    {
        SetImage(texture);
        NormalizedOrigin = Vector2.One;
        ImageScale = 1f;
        _frames = frames;
        _frameTime = frameTime; 
    }

    public void SetImage(Asset<Texture2D> texture)
    {
        _texture = texture;
    }

    protected override float ResolveWidth(CalculatedStyle topMostDimensions)
    {
        return _texture.Value.Frame(1, _frames, 0, _currentFrame).Width;
    }

    protected override float ResolveHeight(CalculatedStyle topMostDimensions)
    {
        return _texture.Value.Frame(1, _frames, 0, _currentFrame).Height;
    }

    public override void Update(GameTime gameTime)
    {
        if (_frameTime > 0)
        {
            _frameCounter++;
            if(_frameCounter >= _frameTime)
            {
                _frameCounter = 0;
                _currentFrame++;
                if (_currentFrame >= _frames)
                    _currentFrame = 0;
            }
        }
		base.Update(gameTime);
	}

    protected override void DrawSelf(SpriteBatchX spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();
        Texture2D texture2D = null;
        if (_texture != null)
            texture2D = _texture.Value;


        Vector2 vector = texture2D.Size();
        Vector2 vector2 = dimensions.Position() + vector * (1f - ImageScale) / 2f + vector * NormalizedOrigin;

        spriteBatch.Draw(texture2D, vector2, texture2D.Frame(1, _frames, 0, _currentFrame), Color, 0f, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
    }
}
