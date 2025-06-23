using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.MissionSystem.Core;
using Luxcinder.Functions.UISystem;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using Luxcinder.Functions.UISystem.UINodes.Layout;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Luxcinder.Functions.MissionSystem.UI;

public class MissionPageFullUI : LuxUIState
{
	private BasicDebugDrawer _debugDrawer;
	private Asset<Texture2D> _missionBorderTexture;

	private MissionContentUI _missionContentUI;
	private LuxUIPanel _backgroundPanel;

	public override void OnInitialize()
	{
		Main.QueueMainThreadAction(() =>
		{
			_debugDrawer = new BasicDebugDrawer(Main.graphics.GraphicsDevice);
		});

		_missionBorderTexture = this.RequestModRelativeTexture("../Assets/UI/MissionBorder");


		_missionContentUI = new MissionContentUI();
		_missionContentUI.Height.SetAuto(true);
		_missionContentUI.Width.Set(0, 1);
		_missionContentUI.MinHeight.Set(200, 0);

		_backgroundPanel = new LuxUIPanel(_missionBorderTexture, 32, 32, 32, 32);
		_backgroundPanel.Width.Set(680, 0f);
		_backgroundPanel.Height.SetAuto(true);
		_backgroundPanel.SetPadding(32);

		var sideView = new LuxUIPanel(TextureAssets.InventoryBack7);
		sideView.Width.Set(0, 1f);
		sideView.Height.Set(300, 0);
		sideView.MarginRight = 16;

		var horizontalSplit = new LuxUIHorizontalSplit(0.33f, sideView, _missionContentUI, true);
		horizontalSplit.Width.Set(0, 1);
		horizontalSplit.Height.SetAuto(true);
		_backgroundPanel.AddChild(horizontalSplit);
		AddChild(_backgroundPanel);
	}

	public void SetMission(Mission mission)
	{
		_missionContentUI.SetMission(mission);
		if (mission != null)
		{
			mission.CheckCanComplete(Main.LocalPlayer);
		}
	}

	public Mission Mission { get => _missionContentUI.Mission; }

	public void Refresh()
	{
		_missionContentUI.RefreshMissionUI();
	}

	public override void Update(GameTime gameTime)
	{
		
		_backgroundPanel.Top.Set(-_backgroundPanel.ResolvedOuterHeight / 2, 0.5f);
		_backgroundPanel.Left.Set(-_backgroundPanel.ResolvedOuterWidth / 2, 0.5f);
		base.Update(gameTime);
	}

	public override void Draw(SpriteBatchX spriteBatch) 
	{
		base.Draw(spriteBatch);

		//if (_debugDrawer != null)
		//{
		//	_debugDrawer.Begin(Main.UIScaleMatrix);
		//	DrawDebugHitbox(_debugDrawer, true);
		//	_debugDrawer.End();
		//}
	}
}

public class MissionPageFullUI_InventoryLayer : LuxcinderUILayer
{
	public override string InterfaceLayerName => "Luxcinder.MissionSystem.MissionPageFull";
	private LuxUI _userInterface;
	private MissionPageFullUI _uiState;

	public MissionPageFullUI_InventoryLayer()
	{
		_userInterface = new LuxUI();
		_uiState = new MissionPageFullUI();
		_userInterface.SetState(_uiState);
	}

	public void SetMission(Mission mission)
	{
		_uiState.SetMission(mission);
	}

	public override void OnActivate()
	{
		var missionPlayer = Main.LocalPlayer.GetModPlayer<MissionPlayer>();
		if(_uiState.Mission == null)
		{
			var mission = missionPlayer.Missions.Values.FirstOrDefault();
			_uiState.SetMission(mission);
		}
	}

	public override void OnDeactivate()
	{
	}

	public override void Update(GameTime gameTime)
	{
		if (IsActive)
		{
			_userInterface.Update(gameTime);
		}

	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (IsActive)
		{
			_userInterface.Draw(spriteBatch, Main._drawInterfaceGameTime);
		}
	}
}