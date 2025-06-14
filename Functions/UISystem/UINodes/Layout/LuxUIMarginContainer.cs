using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxcinder.Functions.UISystem.UICore;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes.Flex;
internal class LuxUIMarginContainer : LuxUIContainer
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

        //Top.Set(MarginTop, 0f);
        //Left.Set(MarginLeft, 0f);
        //Width.Set(-MarginLeft - MarginRight, 1f);
        //Height.Set(-MarginTop - MarginBottom, 1f);
    }

    protected override float ResolveWidth(CalculatedStyle topMostDimensions)
    {
        // If AutoGrow is enabled, we add the margins to the base width calculation.
        if (Width.AutoGrow)
        {
            return base.ResolveWidth(topMostDimensions) + MarginLeft + MarginRight;
        }
        else
        {
            // If AutoGrow is not enabled, we subtract the margins from the parent width calculation.
            float width = topMostDimensions.Width;
            if (Parent != null)
            {
                width = Parent.ResolvedWidth;
            }
            return width - MarginLeft - MarginRight;
        }
    }

    protected override float ResolveHeight(CalculatedStyle topMostDimensions)
    {

        // If AutoGrow is enabled, we add the margins to the base height calculation.
        if (Height.AutoGrow)
        {
            return base.ResolveHeight(topMostDimensions)/*+ MarginTop + MarginBottom*/;
        }
        else
        {
            // If AutoGrow is not enabled, we subtract the margins from the parent height calculation.
            float height = topMostDimensions.Height;
            if (Parent != null)
            {
                height = Parent.ResolvedHeight;
            }
            return height - MarginTop - MarginBottom;
        }
    }

    protected override float ResolveLeft(CalculatedStyle topMostDimensions)
    {
        if (Width.AutoGrow)
        {
            return base.ResolveLeft(topMostDimensions)+ MarginLeft;
        }
        else
        {
            return base.ResolveLeft(topMostDimensions) + MarginLeft;
        }
    }

    protected override float ResolveTop(CalculatedStyle topMostDimensions)
    {
        if (Height.AutoGrow)
        {
            return base.ResolveTop(topMostDimensions) + MarginTop;
        }
        else
        {
            return base.ResolveTop(topMostDimensions) + MarginTop;
        }
    }

}
