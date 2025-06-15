using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;

namespace Luxcinder.Functions.UISystem.UINodes.Layout;
public class LuxUIHorizontalSplit : LuxUIContainer
{
    public LuxUIHorizontalSplit(float splitPoint, LuxUIContainer container1, LuxUIContainer container2)
    {
        var c1 = new LuxUIContainer();
        c1.Width.Set(0, splitPoint);
        c1.Height.Set(0, 1);

        var c2 = new LuxUIContainer();
        c2.Left.Set(0, splitPoint);
        c2.Width.Set(0, 1 - splitPoint);
        c2.Height.Set(0, 1);
        AddChild(c1);
        AddChild(c2);


        if (container1 != null)
            c1.AddChild(container1);
        if (container2 != null)
            c2.AddChild(container2);
    }
}
