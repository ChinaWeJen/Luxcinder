using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
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


		_missionButtonBackground = new LuxUIFramedImage(_missionIconHover, 1, 16);
		_missionButtonBackground.Top.Set(360, 0);
		_missionButtonBackground.Left.Set(520, 0);
		_missionButtonBackground.Visible = false;
		_missionButtonBackground.NormalizedOrigin = Vector2.One * 0.5f;

		_missionButtonBackground.AddChild(new LuxUIAnchor(_missionButtonUI, Vector2.One * 0.5f, Vector2.One * 0.5f));
		AddChild(_missionButtonBackground);
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
		if (true)
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


	//private Vector2 _drawPosition;
 //   private bool _mouseOver;
 //   private bool _isMissionActive;
 //   private int _frameCounter = 0;
 //   private int _frameIndex = 0;

 //   public override void Draw(SpriteBatch spriteBatch)
 //   {
 //       if (Main.playerInventory)
 //       {
 //           if (_mouseOver)
 //           {
 //               spriteBatch.Draw(_missionIconHover.Value, _drawPosition, null, Color.White, 0f, _missionIconHover.Value.Size() * 0.5f, 1f, SpriteEffects.None, 0);
 //           }
 //           if (_isMissionActive)
 //           {
 //               Texture2D texture = _missionIconActive.Value;
 //               Rectangle frame = texture.Frame(1, 4, 0, _frameIndex);
 //               spriteBatch.Draw(texture, _drawPosition, frame, Color.White, 0f, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
 //           }
 //           else
 //           {
 //               spriteBatch.Draw(_missionIcon.Value, _drawPosition, null, Color.White, 0f, _missionIcon.Value.Size() * 0.5f, 1f, SpriteEffects.None, 0);
 //           }
 //       }
 //   }

 //   public override void Update(GameTime gameTime)
 //   {
 //       _mouseOver = false;
 //       if (Main.playerInventory)
 //       {
 //           _drawPosition = new Vector2(520, 360);
 //           _isMissionActive = true;
 //           Rectangle rect = new Rectangle((int)_drawPosition.X - 30, (int)_drawPosition.Y - 30, 60, 60);
 //           PlayerInput.SetZoom_UI();
 //           if (rect.Contains(Main.MouseScreen.ToPoint()))
 //           {
 //               _mouseOver = true;
 //           }

 //           // 动态帧图
 //           _frameCounter++;
 //           if (_frameCounter > 7)
 //           {
 //               _frameCounter = 0;
 //               _frameIndex = (_frameIndex + 1) % 4;
 //           }
 //       }

 //       PlayerInput.SetZoom_World();
 //   }
}
