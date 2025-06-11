using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;
public class LuxUIState : LuxcinderUIBase
{
	public LuxUIState()
	{
		Width.Precent = 1f;
		Height.Precent = 1f;
		Recalculate();
	}
}
