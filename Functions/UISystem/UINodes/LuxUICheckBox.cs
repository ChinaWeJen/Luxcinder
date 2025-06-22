using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Core.Renderer;
using Luxcinder.Functions.UISystem.UICore;
using Luxcinder.Functions.UISystem.Utils;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxUICheckBox : LuxUIContainer
{
	public bool Checked { get; private set; }
	public bool CanInteract { get; set; }
	public LuxUICheckBox()
	{
		Checked = false;
	}

	public void SetChecked(bool value)
	{
		if (Checked != value)
		{
			Checked = value;
		}
	}

	public void Toggle()
	{
		SetChecked(!Checked);
	}

	protected override void DrawSelf(SpriteBatchX spriteBatch)
	{
		var dim = GetDimensions();

		// 绘制外框
		Texture2D checkBoxTexture = UIAssetLocator.CheckBox_Box_Texture.Value;
		Texture2D checkTexture = UIAssetLocator.CheckBox_Check_Texture.Value;

		spriteBatch.Draw(checkBoxTexture, dim.Center(), null, Color.White, 0f, checkBoxTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		// 绘制选中状态
		if (Checked)
		{
			spriteBatch.Draw(checkTexture, dim.Center(), null, Color.White, 0f, checkTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
	}

	public override void LeftClick(LuxUIMouseEvent evt)
	{
		base.LeftClick(evt);
		if (CanInteract)
		{
			Toggle();
		}
	}
}
