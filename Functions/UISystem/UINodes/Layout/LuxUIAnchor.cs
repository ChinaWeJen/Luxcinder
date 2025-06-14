using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;

namespace Luxcinder.Functions.UISystem.UINodes.Layout;
public class LuxUIAnchor : LuxUIContainer
{
	public LuxUIAnchor()
	{
		Width.Set(0, 1);
		Height.Set(0, 1);
	}
    public override void ResolveDependencies()
	{
		var child = Children[0];
		child.Top.Set(-child.ResolvedOuterHeight / 2, 0.5f);
		child.Left.Set(-child.ResolvedOuterWidth / 2, 0.5f);
		base.ResolveDependencies();
	}
}
