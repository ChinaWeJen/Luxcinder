using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.MissionSystem.UI;
using Luxcinder.Functions.UISystem;
using Luxcinder.Functions.UISystem.Example;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Layout;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Luxcinder.Functions.MissionSystem;

public class MissionButtonUIState : LuxUIState
{
	private Asset<Texture2D> _missionIcon;
	private Asset<Texture2D> _missionIconHover;
	private Asset<Texture2D> _missionIconActive;

	private LuxUIFramedImage _missionButtonUI;
	private LuxUIFramedImage _missionButtonBackground;

	public override void OnInitialize()
	{
		_missionIcon = this.RequestModRelativeTexture("Assets/UI/Mission");
		_missionIconHover = this.RequestModRelativeTexture("Assets/UI/Mission_Hover");
		_missionIconActive = this.RequestModRelativeTexture("Assets/UI/Mission_Active");

		_missionButtonUI = new LuxUIFramedImage(_missionIcon, 1, 7);
		_missionButtonUI.NormalizedOrigin = Vector2.One * 0.5f;
		_missionButtonUI.Top.Set(0, 0);
		_missionButtonUI.Left.Set(0, 0);
		_missionButtonUI.OnMouseOut += _missionButtonUI_OnMouseOut;
		_missionButtonUI.OnMouseOver += _missionButtonUI_OnMouseOver;
		_missionButtonUI.OnLeftClick += _missionButtonUI_OnLeftClick;


		_missionButtonBackground = new LuxUIFramedImage(_missionIconHover, 1, 16);
		_missionButtonBackground.Top.Set(360, 0);
		_missionButtonBackground.Left.Set(520, 0);
		_missionButtonBackground.Visible = false;
		_missionButtonBackground.NormalizedOrigin = Vector2.One * 0.5f;

		_missionButtonBackground.AddChild(new LuxUIAnchor(_missionButtonUI, Vector2.One * 0.5f, Vector2.One * 0.5f));
		AddChild(_missionButtonBackground);
	}

	private void _missionButtonUI_OnLeftClick(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
	{
		LuxUISystem.Toggle<MissionPageFullUI_InventoryLayer>();
	}

	private void _missionButtonUI_OnMouseOver(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
	{ 
		_missionButtonBackground.Visible = true;
	}

	private void _missionButtonUI_OnMouseOut(LuxUIMouseEvent evt, LuxUIContainer listeningElement)
	{
		_missionButtonBackground.Visible = false;
	}

	public override void Update(GameTime gameTime)
	{
		var missionPlayer = Main.LocalPlayer.GetModPlayer<MissionPlayer>();
		if (missionPlayer.HasIncompletedMission())
		{
			_missionButtonUI.SetImage(_missionIconActive);
			_missionButtonUI.Frames = 4;
			_missionButtonUI.FrameTime = 8;
		}
		else
		{
			_missionButtonUI.SetImage(_missionIcon);
			_missionButtonUI.Frames = 1;
		}
		if (_missionButtonUI.IsMouseHovering)
		{
			Main.LocalPlayer.mouseInterface = true;
		}
		base.Update(gameTime);
	}
}

public class MissionButtonUI_InventoryLayer : LuxcinderUILayer
{
	public override string InterfaceLayerName => "Luxcinder.MissionSystem.InventoryButtonUI";
	private LuxUI _userInterface;
	private MissionButtonUIState _uiState;

	public MissionButtonUI_InventoryLayer()
	{
		_userInterface = new LuxUI();
		_uiState = new MissionButtonUIState();
		_userInterface.SetState(_uiState);
	}

	public override void Update(GameTime gameTime)
	{
		if (Main.playerInventory)
		{
			_userInterface.Update(gameTime);
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (Main.playerInventory)
		{
			_userInterface.Draw(spriteBatch, Main._drawInterfaceGameTime);
		}
	}
}
