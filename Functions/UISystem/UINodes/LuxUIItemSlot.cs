using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUIItemSlot : LuxUIContainer
{
	public static Asset<Texture2D> defaultBackgroundTexture = TextureAssets.InventoryBack9;

	public Asset<Texture2D> backgroundTexture = defaultBackgroundTexture;

	private float _scale = 0.75f;

	private Item _item;

	private bool _hideSlot;


	public LuxUIItemSlot(Item item, float scale = 0.75f)
	{
		this._scale = scale;
		this._item = item;
	}

	protected override float ResolveWidth(CalculatedStyle topMostDimensions)
	{
		return defaultBackgroundTexture.Width() * _scale;
	}

	protected override float ResolveHeight(CalculatedStyle topMostDimensions)
	{
		return defaultBackgroundTexture.Height() * _scale;
	}

	internal static void LoadItem(int type)
	{
		if (TextureAssets.Item[type].State == 0)
		{
			Main.Assets.Request<Texture2D>(TextureAssets.Item[type].Name, (AssetRequestMode)2);
		}
	}


	protected override void DrawSelf(SpriteBatchX spriteBatch)
	{
		if (_item == null)
		{
			return;
		}
		CalculatedStyle dimensions = GetInnerDimensions();
		dimensions.ToRectangle();
		if (!_hideSlot)
		{
			spriteBatch.Draw(backgroundTexture.Value, dimensions.Position(),null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
			DrawAdditionalOverlays(spriteBatch, dimensions.Position(), _scale);
		}
		if (_item.IsAir)
		{
			return;
		}
		LoadItem(_item.type);
		Texture2D itemTexture = TextureAssets.Item[_item.type].Value;
		DrawAnimation animation = Main.itemAnimations[_item.type];
		Rectangle rectangle2 = ((animation != null) ? animation.GetFrame(itemTexture, -1) : itemTexture.Frame());
		Color newColor = Color.White;
		float pulseScale = 1f;
		ItemSlot.GetItemLight(ref newColor, ref pulseScale, _item, false);
		int height = rectangle2.Height;
		int width = rectangle2.Width;
		float drawScale = 1f;
		float availableWidth = defaultBackgroundTexture.Width() * _scale;
		if (width > availableWidth || height > availableWidth)
		{
			drawScale = ((width <= height) ? (availableWidth / height) : (availableWidth / width));
		}
		drawScale *= _scale;
		Vector2 size = backgroundTexture.Size() * _scale;
		Vector2 position2 = dimensions.Position() + size / 2f;
		Vector2 origin = rectangle2.Size() / 2f;
		if (ItemLoader.PreDrawInInventory(_item, spriteBatch.WrappedSpriteBatch, position2, rectangle2, _item.GetAlpha(newColor), _item.GetColor(Color.White), origin, drawScale * pulseScale))
		{
			spriteBatch.Draw(itemTexture, position2, (Rectangle?)rectangle2, _item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, (SpriteEffects)0, 0f);
			if (_item.color != Color.Transparent)
			{
				spriteBatch.Draw(itemTexture, position2, (Rectangle?)rectangle2, _item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, (SpriteEffects)0, 0f);
			}
		}
		ItemLoader.PostDrawInInventory(_item, spriteBatch.WrappedSpriteBatch, position2, rectangle2, _item.GetAlpha(newColor), _item.GetColor(Color.White), origin, drawScale * pulseScale);
		if (Terraria.ID.ItemID.Sets.TrapSigned[_item.type])
		{
			spriteBatch.Draw(TextureAssets.Wire.Value, dimensions.Position() + new Vector2(40f, 40f) * _scale, (Rectangle?)new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, (SpriteEffects)0, 0f);
		}
		DrawAdditionalBadges(spriteBatch, dimensions.Position(), _scale);
		if (_item.stack > 1)
		{
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch.WrappedSpriteBatch, FontAssets.ItemStack.Value, _item.stack.ToString(), dimensions.Position() + new Vector2(10f, 26f) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
		}
		if (this.IsMouseHovering)
		{
			Main.HoverItem = _item.Clone();
			Main.hoverItemName = Main.HoverItem.Name;
			Main.HoverItem.SetNameOverride(Main.HoverItem.Name);
			Main.instance.MouseText(_item.Name, _item.rare, 0);
		}
	}

	internal virtual void DrawAdditionalOverlays(SpriteBatchX spriteBatch, Vector2 vector2, float scale)
	{
	}

	internal virtual void DrawAdditionalBadges(SpriteBatchX spriteBatch, Vector2 vector2, float scale)
	{
	}
}

