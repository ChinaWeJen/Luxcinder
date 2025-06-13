using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using ReLogic.Content;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUIPanel : LuxcinderUIBase
{
    private int _cornerTop = 12;
    private int _cornerLeft = 12;
    private int _cornerRight = 12;
    private int _cornerBottom = 12;

    private Asset<Texture2D> _panel9GridTexture;
    public Color Color
    {
        get;
        set;
    }


    public LuxUIPanel(Asset<Texture2D> panel9GridTexture, int cornerSizeTop = 12, int cornerSizeLeft = 12, int cornerSizeRight = 12, int cornerSizeBottom = 12)
    {
        Color = Color.White;
        _panel9GridTexture = panel9GridTexture;
        _cornerTop = cornerSizeTop;
        _cornerLeft = cornerSizeLeft;
        _cornerRight = cornerSizeRight;
        _cornerBottom = cornerSizeBottom;
    }

    private void DrawPanel(SpriteBatchX spriteBatch, Texture2D texture, Color color)
    {
        CalculatedStyle dimensions = GetDimensions();
        Point point = new Point((int)dimensions.X, (int)dimensions.Y);
        Point point2 = new Point(point.X + (int)dimensions.Width - _cornerRight, point.Y + (int)dimensions.Height - _cornerBottom);
        int width = point2.X - point.X - _cornerLeft;
        int height = point2.Y - point.Y - _cornerTop;

        // 以九宫格的形式绘制填充
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, _cornerLeft, _cornerTop), new Rectangle(0, 0, _cornerLeft, _cornerTop), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, _cornerRight, _cornerTop), new Rectangle(texture.Width - _cornerRight, 0, _cornerRight, _cornerTop), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, _cornerLeft, _cornerBottom), new Rectangle(0, texture.Height - _cornerBottom, _cornerLeft, _cornerBottom), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, _cornerRight, _cornerBottom), new Rectangle(texture.Width - _cornerRight, texture.Height - _cornerBottom, _cornerRight, _cornerBottom), color);

        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerLeft, point.Y, width, _cornerTop), new Rectangle(_cornerLeft, 0, texture.Width - _cornerLeft - _cornerRight, _cornerTop), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerLeft, point2.Y, width, _cornerBottom), new Rectangle(_cornerLeft, texture.Height - _cornerBottom, texture.Width - _cornerLeft - _cornerRight, _cornerBottom), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + _cornerTop, _cornerLeft, height), new Rectangle(0, _cornerTop, _cornerLeft, texture.Height - _cornerTop - _cornerBottom), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + _cornerTop, _cornerRight, height), new Rectangle(texture.Width - _cornerRight, _cornerTop, _cornerRight, texture.Height - _cornerTop - _cornerBottom), color);

        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerLeft, point.Y + _cornerTop, width, height), new Rectangle(_cornerLeft, _cornerTop, texture.Width - _cornerLeft - _cornerRight, texture.Height - _cornerTop - _cornerBottom), color);
    }

    protected override void DrawSelf(SpriteBatchX spriteBatch)
    {
        if (_panel9GridTexture != null)
            DrawPanel(spriteBatch, _panel9GridTexture.Value, Color);
    }
}
