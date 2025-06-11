using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;
public class LuxUIEvent
{
	/// <summary>
	/// The element targeted by the mouse event. This is usually the element lowest in the UI hierarchy overlapping with mouse position unless <see cref="UIElement.IgnoresMouseInteraction"/> is true.
	/// </summary>
	public readonly LuxcinderUIBase Target;

	public LuxUIEvent(LuxcinderUIBase target)
	{
		Target = target;
	}
}


public class LuxUIMouseEvent : LuxUIEvent
{
	public readonly Vector2 MousePosition;

	public LuxUIMouseEvent(LuxcinderUIBase target, Vector2 mousePosition)
		: base(target)
	{
		MousePosition = mousePosition;
	}
}

public class LuxUIScrollWheelEvent : LuxUIMouseEvent
{
	public readonly int ScrollWheelValue;

	public LuxUIScrollWheelEvent(LuxcinderUIBase target, Vector2 mousePosition, int scrollWheelValue)
		: base(target, mousePosition)
	{
		ScrollWheelValue = scrollWheelValue;
	}
}
