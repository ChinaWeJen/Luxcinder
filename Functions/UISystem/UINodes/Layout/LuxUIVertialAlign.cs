using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes.Layout;
public class LuxUIVertialAlign : LuxUIContainer
{	
	protected override float ResolveHeight(CalculatedStyle topMostDimensions)
	{
		float height = 0;
		foreach (var child in Children)
		{
			height += child.ResolvedOuterHeight;
		}
		return height + MarginTop + MarginBottom + PaddingTop + PaddingBottom;
	}

	public override void RecalculateChildren()
	{
        float curHeight = 0;
        foreach (var child in Children)
        {
			child.Top.Set(curHeight, 0);
            curHeight += child.ResolvedOuterHeight;
        }
        base.RecalculateChildren();
	}

	public void ClearChildren()
	{
		foreach(var child in Children)
		{
			child.Parent = null;
		}
		Children.Clear();
	}
}
