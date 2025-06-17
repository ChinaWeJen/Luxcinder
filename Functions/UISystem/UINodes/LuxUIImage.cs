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
public class LuxUIImage : LuxUIContainer
{
    private Asset<Texture2D> _texture;
    public float ImageScale
    {
        get;
        set;
    }
    public float Rotation
    {
        get;
        set;
    }
    public bool ScaleToFit
    {
        get;
        set;
    }
    public bool AllowResizingDimensions
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

    public bool RemoveFloatingPointsFromDrawPosition
    {
        get;
        set;
    }

    public LuxUIImage(Asset<Texture2D> texture)
    {
        SetImage(texture);
        NormalizedOrigin = Vector2.One;
        AllowResizingDimensions = true;
        ScaleToFit = false;
        ImageScale = 1f;
        Rotation = 0f;
        RemoveFloatingPointsFromDrawPosition = false;
    }

    public void SetImage(Asset<Texture2D> texture)
    {
        _texture = texture;
    }

    protected override float ResolveWidth(CalculatedStyle topMostDimensions)
    {
        if (AllowResizingDimensions)
        {
            return _texture.Value.Width;
        }
        return base.ResolveWidth(topMostDimensions);
    }

    protected override float ResolveHeight(CalculatedStyle topMostDimensions)
    {
        if (AllowResizingDimensions)
        {
            return _texture.Value.Height;
        }
        return base.ResolveHeight(topMostDimensions);
    }

    protected override void DrawSelf(SpriteBatchX spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();
        Texture2D texture2D = null;
        if (_texture != null)
            texture2D = _texture.Value;

        if (ScaleToFit)
        {
            spriteBatch.Draw(texture2D, dimensions.ToRectangle(), Color);
            return;
        }

        Vector2 vector = new Vector2(dimensions.Width, dimensions.Height);
        Vector2 vector2 = dimensions.Position() + vector * NormalizedOrigin;
        if (RemoveFloatingPointsFromDrawPosition)
            vector2 = vector2.Floor();

        spriteBatch.Draw(texture2D, vector2, null, Color, Rotation, texture2D.Size() * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
    }
}
