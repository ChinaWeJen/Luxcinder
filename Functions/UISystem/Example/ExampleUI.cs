using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.NPCChat;
using Luxcinder.Functions.NPCChat.Nodes;
using Luxcinder.Functions.UISystem.UICore;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.Example;
public class ExampleUI : LuxcinderUILayer
{
	private LuxUI _userInterface;
	private ExampleUIState _exampleUIState;

	public static ExampleUI Instance { get; private set; }

	public override string InterfaceLayerName => "Luxcinder.ExampleUI.ExampleUI";

	public ExampleUI()
	{
		Instance = this;
        _userInterface = new LuxUI();
		_exampleUIState = new ExampleUIState();
	}

	public void Activate()
	{
        _exampleUIState = new ExampleUIState();
        _userInterface.SetState(_exampleUIState);
	}

	public override void Update(GameTime gameTime)
	{
		_userInterface.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		_userInterface.Draw(spriteBatch, Main._drawInterfaceGameTime);

	}

	public void Deactivate()
	{
		_userInterface.SetState(null);
	}
}