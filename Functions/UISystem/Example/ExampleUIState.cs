using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Flex;
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
			LuxUIMarginContainer marginContainer = new LuxUIMarginContainer(10, 10, 10, 10);
            LuxcinderUIBase grid = new LuxcinderUIBase();
			grid.Width.Set(0, 0.5f);
			grid.Height.Set(0, 0.5f);

			grid.Top.Set(0, 0);
			grid.Left.Set(0, 0);
			panel.AddChild(grid);

			grid.AddChild(marginContainer);
        }

		LuxUIPanel panel2 = new LuxUIPanel(_backgroundTexture, 32, 32, 32, 32);
		panel2.Top.Set(500, 0f);
		panel2.Left.Set(800, 0f);
		panel2.Width.Set(300, 0f);
		panel2.Height.Set(200, 0f);

		LuxUIText text = new LuxUIText("Example UI State");
		panel2.AddChild(text);
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
