using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;

namespace Luxcinder.Functions.UISystem.UINodes.Flex;
public class LuxUISplit4Container : LuxcinderUIBase
{
	public override void InitializeDependencies()
	{
		for (int i = 0; i < Math.Min(4, Children.Count); i++)
		{
			var child = Children[i];
            child.Width.Set(0, 0.5f);
            child.Height.Set(0, 0.5f);
			if (i == 0)
			{
				child.Top.Set(0, 0);
				child.Left.Set(0, 0);
			}
			else if (i == 1)
			{
                child.Top.Set(0, 0);
                child.Left.Set(0, 0.5f);
            }
            else if (i == 2)
            {
                child.Top.Set(0, 0.5f);
                child.Left.Set(0, 0);
            }
            else if (i == 3)
            {
                child.Top.Set(0, 0.5f);
                child.Left.Set(0, 0.5f);
            }
        }
        base.InitializeDependencies();
	}
}
