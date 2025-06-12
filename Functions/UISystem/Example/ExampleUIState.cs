using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes.Flex;
using ReLogic.Graphics;

namespace Luxcinder.Functions.UISystem.Example;
internal class ExampleUIState : LuxUIState
{
	private BasicDebugDrawer _debugDrawer;
	public ExampleUIState()
	{
		Main.QueueMainThreadAction(() =>
		{
			_debugDrawer = new BasicDebugDrawer(Main.graphics.GraphicsDevice);
		});

        LuxUISplit4Container panel = new LuxUISplit4Container();
		panel.Top.Set(500, 0f);
		panel.Left.Set(500, 0f);
		panel.Width.Set(200, 0f);
		panel.Height.Set(200, 0f);
		
		AddChild(panel);

	
		for (int i = 0; i < 4; i++)
		{
            LuxcinderUIBase grid = new LuxcinderUIBase();
			grid.Width.Set(0, 0.5f);
			grid.Height.Set(0, 0.5f);

			grid.Top.Set(0, 0);
			grid.Left.Set(0, 0);
			panel.AddChild(grid);
        }
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
        _debugDrawer.Begin(Main.UIScaleMatrix);
        DrawDebugHitbox(_debugDrawer, true);
		_debugDrawer.End();
	}
}
