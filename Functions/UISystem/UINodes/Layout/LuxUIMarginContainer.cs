using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;

namespace Luxcinder.Functions.UISystem.UINodes.Flex;
internal class LuxUIMarginContainer : LuxcinderUIBase
{
    public float MarginLeft;
    public float MarginRight;
    public float MarginTop;
    public float MarginBottom;

    public LuxUIMarginContainer(float left, float right, float top, float bottom)
    {
        MarginLeft = left;
        MarginRight = right;
        MarginTop = top;
        MarginBottom = bottom;

        Top.Set(MarginTop, 0f);
        Left.Set(MarginLeft, 0f);
        Width.Set(-MarginLeft - MarginRight, 1f);
        Height.Set(-MarginTop - MarginBottom, 1f);
    }
}
