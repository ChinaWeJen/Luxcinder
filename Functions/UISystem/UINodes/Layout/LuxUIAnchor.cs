using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;

namespace Luxcinder.Functions.UISystem.UINodes.Layout;
public class LuxUIAnchor : LuxUIContainer
{
	private Vector2 _anchor;
	private Vector2 _origin;
	public LuxUIAnchor(LuxUIContainer element, Vector2 anchor, Vector2 origin)
	{
		Width.Set(0, 1);
		Height.Set(0, 1);
		_anchor = anchor;
		_origin = origin;
		AddChild(element);
	}
    public override void ResolveDependencies()
	{
		var child = Children[0];
		child.Left.Set(-child.ResolvedOuterWidth * _origin.X, _anchor.X);
		child.Top.Set(-child.ResolvedOuterHeight * _origin.Y, _anchor.Y);
		base.ResolveDependencies();
	}
}
