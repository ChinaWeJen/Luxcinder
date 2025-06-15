using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Flex;
using Luxcinder.Functions.UISystem.UINodes.Layout;
using ReLogic.Content;
using ReLogic.Graphics;

namespace Luxcinder.Functions.UISystem.Example;
internal class ExampleUIState : LuxUIState
{
	private BasicDebugDrawer _debugDrawer;
	private Asset<Texture2D> _backgroundTexture;
    public ExampleUIState()
	{
		Main.QueueMainThreadAction(() =>
		{
			_debugDrawer = new BasicDebugDrawer(Main.graphics.GraphicsDevice);
		});

        _backgroundTexture = this.RequestModRelativeTexture("panel");

        LuxUISplit4Container panel = new LuxUISplit4Container();
		panel.Top.Set(500, 0f);
		panel.Left.Set(500, 0f);
		panel.Width.Set(200, 0f);
		panel.Height.Set(200, 0f);
		
		AddChild(panel);

	
		for (int i = 0; i < 4; i++)
		{
            LuxUIContainer grid = new LuxUIContainer();
			grid.PaddingBottom = 10;
			grid.PaddingLeft = 10;
			grid.PaddingTop = 10;
			grid.PaddingRight = 10;

			grid.Width.Set(0, 0.5f);
			grid.Height.Set(0, 0.5f);

			grid.Top.Set(0, 0);
			grid.Left.Set(0, 0);

			LuxUIImage image = new LuxUIImage(this.RequestModRelativeTexture("UIViewer"));
            grid.AddChild(image);
            panel.AddChild(grid);

        }

		LuxUIPanel panel2 = new LuxUIPanel(_backgroundTexture, 32, 32, 32, 32);
		panel2.Top.Set(500, 0f);
		panel2.Left.Set(800, 0f);
		panel2.Width.Set(300, 0f);
		panel2.Height.SetAuto(true);
		panel2.MinHeight.Set(200, 0f);

        panel2.PaddingBottom = 32;
        panel2.PaddingLeft = 32;
        panel2.PaddingTop = 32;
        panel2.PaddingRight = 32;

        LuxUIVertialAlign luxUIVertialAlign = new LuxUIVertialAlign();
		luxUIVertialAlign.Width.Set(0, 1f);
		luxUIVertialAlign.Height.SetAuto(true);
        panel2.AddChild(luxUIVertialAlign);

	

        LuxUIText text = new LuxUIText("那个...我第一次来到这个地方");
		text.TextLayout = TextLayout.AutoWrap;
		text.Width.Set(0, 1);

        LuxUIText text2 = new LuxUIText("选项1，选项2，选项3");
        text2.TextLayout = TextLayout.AutoWrap;
        text2.Width.Set(0, 1);
        luxUIVertialAlign.AddChild(text);
        luxUIVertialAlign.AddChild(text2);


        LuxUIVertialAlign luxUIVertialAlign2 = new LuxUIVertialAlign();
        luxUIVertialAlign2.Width.Set(0, 1f);
        luxUIVertialAlign2.Height.SetAuto(true);
		luxUIVertialAlign2.MarginTop = 20;
		luxUIVertialAlign.AddChild(luxUIVertialAlign2);


		for (int i = 0; i < 3; i++)
		{
            LuxUIText textOpt = new LuxUIText("选项1，324423423423423424234221434324324");
            textOpt.TextLayout = TextLayout.AutoWrap;
            textOpt.Width.Set(0, 1);
			luxUIVertialAlign2.AddChild(textOpt);
        }


        var centerIcon = new LuxUIFramedImage(this.RequestModRelativeTexture("NextStep"), 1, 6);
		var anchor = new LuxUIAnchor(centerIcon, Vector2.One * 0.5f, Vector2.One * 0.5f);
		centerIcon.NormalizedOrigin = Vector2.One * 0.5f;
		centerIcon.OnMouseOver += (sender, args) =>
		{
			centerIcon.SetImage(this.RequestModRelativeTexture("NextStep_Hover"));
			centerIcon.Frames = 7;
        };
		centerIcon.OnMouseOut += (sender, args) =>
		{
            centerIcon.SetImage(this.RequestModRelativeTexture("NextStep"));
            centerIcon.Frames = 1; 
		};
        LuxUIHorizontalSplit luxUIHorizontalSplit = new LuxUIHorizontalSplit(0.8f, null, anchor);
		luxUIHorizontalSplit.Height.Set(35, 0);
		luxUIHorizontalSplit.Width.Set(0, 1);

        luxUIVertialAlign.AddChild(luxUIHorizontalSplit);

        AddChild(panel2);
	}

	public override void Draw(SpriteBatchX spriteBatch)
	{
		base.Draw(spriteBatch);
        _debugDrawer.Begin(Main.UIScaleMatrix);
        DrawDebugHitbox(_debugDrawer, true);
		_debugDrawer.End();
	}
}
