using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UINodes;
public class LuxcinderUIHorizontalAlign : LuxcinderUIElement
{
	public float PaddingHorizontal { get; private set; }

    public LuxcinderUIHorizontalAlign(float paddingHorizontal = 0f)
    {
        this.PaddingHorizontal = paddingHorizontal;
    }

    public override void RecalculateChildren()
    {
        base.RecalculateChildren();

        float currentLeft = 0;
        float maxHeight = 0;
        for(int i = 0; i < this.Elements.Count; i++)
        {
            var child = this.Elements[i];

            var dimension = child.GetOuterDimensions();
            // Align the child horizontally
            child.Left.Set(currentLeft, 0f);
            child.Top.Set(0, 0f);
            maxHeight = Math.Max(maxHeight, dimension.Height);

            currentLeft += dimension.Width + this.PaddingHorizontal;
        }

        Width.Set(currentLeft, 0f);
        Height.Set(maxHeight, 0f);

        for (int i = 0; i < this.Elements.Count; i++)
        {
            var child = this.Elements[i];

            var dimension = child.GetOuterDimensions();
            child.Top.Set(maxHeight / 2 - dimension.Height / 2, 0);
        }
    }
}
