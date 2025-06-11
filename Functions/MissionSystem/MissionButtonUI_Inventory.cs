using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Luxcinder.Functions.MissionSystem;
internal class MissionButtonUI_Inventory : LuxcinderUILayer
{
	public override string InterfaceLayerName => "Luxcinder.MissionSystem.InventoryButtonUI";

	private Asset<Texture2D> _missionIcon;
	private Asset<Texture2D> _missionIconHover;
	private Asset<Texture2D> _missionIconActive;

    public override void Initialize()
	{
        _missionIcon = this.RequestModRelativeTexture("Assets/UI/Mission");
        _missionIconHover = this.RequestModRelativeTexture("Assets/UI/Mission_Hover");
        _missionIconActive = this.RequestModRelativeTexture("Assets/UI/Mission_Active");
    }

    private Vector2 _drawPosition;
    private bool _mouseOver;
    private bool _isMissionActive;
    private int _frameCounter = 0;
    private int _frameIndex = 0;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.playerInventory)
        {
            if (_mouseOver)
            {
                spriteBatch.Draw(_missionIconHover.Value, _drawPosition, null, Color.White, 0f, _missionIconHover.Value.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            if (_isMissionActive)
            {
                Texture2D texture = _missionIconActive.Value;
                Rectangle frame = texture.Frame(1, 4, 0, _frameIndex);
                spriteBatch.Draw(texture, _drawPosition, frame, Color.White, 0f, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(_missionIcon.Value, _drawPosition, null, Color.White, 0f, _missionIcon.Value.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        _mouseOver = false;
        if (Main.playerInventory)
        {
            _drawPosition = new Vector2(520, 360);
            _isMissionActive = true;
            Rectangle rect = new Rectangle((int)_drawPosition.X - 30, (int)_drawPosition.Y - 30, 60, 60);
            PlayerInput.SetZoom_UI();
            if (rect.Contains(Main.MouseScreen.ToPoint()))
            {
                _mouseOver = true;
            }

            // 动态帧图
            _frameCounter++;
            if (_frameCounter > 7)
            {
                _frameCounter = 0;
                _frameIndex = (_frameIndex + 1) % 4;
            }
        }

        PlayerInput.SetZoom_World();
    }
}
