using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.MissionSystem.Core;
using Luxcinder.Functions.UISystem;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.UINodes;
using ReLogic.Content;
using ReLogic.Graphics;

namespace Luxcinder.Functions.MissionSystem.UI;

public class MissionPageUI : LuxUIState
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
		_backgroundPanel.Width.Set(500, 0f);
		_backgroundPanel.Height.SetAuto(true);
		_backgroundPanel.SetPadding(32);
		_backgroundPanel.AddChild(_missionContentUI);

		AddChild(_backgroundPanel);
	}

	public void SetMission(Mission mission)
	{
		_missionContentUI.SetMission(mission);
	}

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

		if (_debugDrawer != null)
		{
			_debugDrawer.Begin(Main.UIScaleMatrix);
			DrawDebugHitbox(_debugDrawer, true);
			_debugDrawer.End();
		}
	}
}

public class MissionPageUI_InventoryLayer : LuxcinderUILayer
{
	public override string InterfaceLayerName => "Luxcinder.MissionSystem.MissionPage";
	private LuxUI _userInterface;
	private MissionPageUI _uiState;

	public MissionPageUI_InventoryLayer()
	{
		_userInterface = new LuxUI();
		_uiState = new MissionPageUI();
		_userInterface.SetState(_uiState);
	}

	public void SetMission(Mission mission)
	{
		_uiState.SetMission(mission);
	}

	public override void OnActivate()
	{
		_uiState.Refresh();
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