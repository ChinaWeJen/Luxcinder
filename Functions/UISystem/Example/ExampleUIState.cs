using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
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

		LuxcinderUIBase panel = new LuxcinderUIBase();
		panel.Top.Set(500, 0f);
		panel.Left.Set(500, 0f);
		panel.Width.Set(200, 0f);
		panel.Height.Set(200, 0f);
		
		AddChild(panel);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		_debugDrawer.Begin();
		base.Draw(spriteBatch);
		DrawDebugHitbox(_debugDrawer, true);
		_debugDrawer.End();
	}
}
