using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes.Layout;
public class LuxUIHorizontalAlign : LuxUIContainer
{
	private bool _isAutoHeight;
	public LuxUIHorizontalAlign(bool autoHeight = false)
	{
		_isAutoHeight = autoHeight;
	}
	protected override float ResolveWidth(CalculatedStyle topMostDimensions)
	{
		float width = topMostDimensions.Width;
		if (Parent != null)
		{
			if (Width.Percent > 0f)
			{
				width = Parent.ResolvedInnerWidth;
			}
		}
		float maxWidth = MaxWidth.GetValue(width);
		float minWidth = MinWidth.GetValue(width);
		float w = 0;
		foreach (var child in Children)
		{
			w += child.ResolvedOuterWidth;
		}
		return MathHelper.Clamp(w + PaddingLeft + PaddingRight + MarginLeft + MarginRight, minWidth, maxWidth);
	}
	protected override float ResolveHeight(CalculatedStyle topMostDimensions)
	{
		if (_isAutoHeight)
		{
			float height = 0;
			foreach (var child in Children)
			{
				height = Math.Max(height, child.ResolvedOuterHeight);
			}
			return height + MarginTop + MarginBottom + PaddingTop + PaddingBottom;
		}
		else
		{
			return base.ResolveHeight(topMostDimensions);
		}
	}

	public override void RecalculateChildren()
	{
		float curLeft = 0;
		foreach (var child in Children)
		{
			child.Left.Set(curLeft, 0);
			curLeft += child.ResolvedOuterWidth;
		}
		base.RecalculateChildren();
	}

	public void ClearChildren()
	{
		foreach (var child in Children)
		{
			child.Parent = null;
		}
		Children.Clear();
	}
}
