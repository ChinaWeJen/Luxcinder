//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Luxcinder.Functions.UISystem.UICore;
//using Terraria.UI;

//namespace Luxcinder.Functions.UISystem.UINodes.Flex;

//public enum FlexDirection
//{
//    Row, Column
//}
//public enum FlexWrap
//{
//    NoWrap, Wrap
//}
//public enum JustifyContent
//{
//    FlexStart, Center, FlexEnd, SpaceBetween, SpaceAround
//}
//public enum AlignItems
//{
//    FlexStart, Center, FlexEnd, Stretch
//}

//public class LuxcinderUIFlexContainer : LuxcinderUIBase
//{
//    public FlexDirection Direction { get; set; } = FlexDirection.Row;
//    public FlexWrap Wrap { get; set; } = FlexWrap.NoWrap; // Note: Wrap functionality is not implemented in this basic version.
//    public JustifyContent JustifyContent { get; set; } = JustifyContent.FlexStart;
//    public AlignItems AlignItems { get; set; } = AlignItems.Stretch; // Default to Stretch
//    public float Gap { get; set; } = 0f;

//    // Flex item properties (can be added to LuxcinderUIBase or managed via attached properties if a more advanced system is needed)
//    // For now, we'll rely on existing Width/Height of children as their base size.
//    // public float FlexGrow { get; set; } // Example for future extension
//    // public float FlexShrink { get; set; } // Example for future extension
//    // public StyleDimension FlexBasis { get; set; } // Example for future extension

//    public LuxcinderUIFlexContainer() : base()
//    {
//        // Specific initialization for FlexContainer if needed
//    }

//    public override void RecalculateChildren()
//    {
//        // base.RecalculateChildren(); // Do not call base, as flex container dictates children's layout entirely.

//        if (!Children.Any())
//        {
//            return;
//        }

//        CalculatedStyle innerDim = GetInnerDimensions(); // Inner dimensions of this flex container

//        // Step 1: Collect preferred sizes and margins of children.
//        // Children's Width/Height DependencyProperties should have been resolved by LayoutSolver
//        // based on their StyleDimensions (e.g., pixels, or % of this container's innerDim).
//        // Call Recalculate on each child to ensure its own dimensions (and thus margins) are up-to-date based on its StyleDimension.
//        foreach (var child in Children)
//        {
//            // Ensure child's own dependencies (like width/height if % of parent) are resolved
//            // This should ideally be handled by the LayoutSolver pass before this Recalculate,
//            // but an explicit Resolve + Recalculate here ensures child's base size is known.
//            child.ResolveDependencies();
//            child.Recalculate();
//        }

//        // For NoWrap, all items are on a single line.
//        // Future extension: Implement line wrapping logic here if FlexWrap.Wrap is enabled.

//        // Step 2: Calculate sizes and total main axis space needed by children
//        List<float> mainSizes = new List<float>();
//        List<float> crossSizes = new List<float>();
//        List<LuxcinderUIBase> lineChildren = new List<LuxcinderUIBase>(Children); // Assuming NoWrap for now

//        float totalMainSizeConsumed = 0;

//        foreach (var child in lineChildren)
//        {
//            CalculatedStyle outerDim = child.GetOuterDimensions();
//            if (Direction == FlexDirection.Row)
//            {
//                mainSizes.Add(outerDim.Width);
//                crossSizes.Add(outerDim.Height);
//                totalMainSizeConsumed += outerDim.Width;
//            }
//            else // Column
//            {
//                mainSizes.Add(outerDim.Height);
//                crossSizes.Add(outerDim.Width);
//                totalMainSizeConsumed += outerDim.Height;
//            }
//        }

//        if (lineChildren.Count > 0)
//        {
//            totalMainSizeConsumed += (lineChildren.Count - 1) * Gap;
//        }

//        // Step 3: Determine main axis starting position and actual gap based on JustifyContent
//        float currentMainPos = 0;
//        float actualGap = Gap;
//        float containerMainAxisSize = (Direction == FlexDirection.Row) ? innerDim.Width : innerDim.Height;

//        switch (JustifyContent)
//        {
//            case JustifyContent.FlexStart:
//                currentMainPos = 0;
//                break;
//            case JustifyContent.Center:
//                currentMainPos = (containerMainAxisSize - totalMainSizeConsumed) / 2f;
//                break;
//            case JustifyContent.FlexEnd:
//                currentMainPos = containerMainAxisSize - totalMainSizeConsumed;
//                break;
//            case JustifyContent.SpaceBetween:
//                currentMainPos = 0;
//                if (lineChildren.Count > 1)
//                {
//                    float totalItemMainSize = mainSizes.Sum();
//                    actualGap = (containerMainAxisSize - totalItemMainSize) / (lineChildren.Count - 1);
//                }
//                else
//                {
//                    actualGap = 0; // Or center the single item: currentMainPos = (containerMainAxisSize - mainSizes.FirstOrDefault()) / 2f;
//                }
//                break;
//            case JustifyContent.SpaceAround:
//                if (lineChildren.Count > 0)
//                {
//                    float totalItemMainSize = mainSizes.Sum();
//                    actualGap = (containerMainAxisSize - totalItemMainSize) / lineChildren.Count;
//                    currentMainPos = actualGap / 2f;
//                }
//                else
//                {
//                    actualGap = 0;
//                    currentMainPos = 0;
//                }
//                break;
//        }

//        // Step 4: Position children
//        for (int i = 0; i < lineChildren.Count; i++)
//        {
//            LuxcinderUIBase child = lineChildren[i];
//            CalculatedStyle childOuterDim = child.GetOuterDimensions(); // Re-get in case Recalculate changed it, though unlikely here.
//            float childMainSize = (Direction == FlexDirection.Row) ? childOuterDim.Width : childOuterDim.Height;
//            float childCrossSize = (Direction == FlexDirection.Row) ? childOuterDim.Height : childOuterDim.Width;

//            float childFinalX, childFinalY;
//            float childFinalWidth = childOuterDim.Width; // Start with child's preferred width
//            float childFinalHeight = childOuterDim.Height; // Start with child's preferred height

//            // Main axis positioning
//            if (Direction == FlexDirection.Row)
//            {
//                childFinalX = currentMainPos + child.MarginLeft;
//            }
//            else // Column
//            {
//                childFinalY = currentMainPos + child.MarginTop;
//            }

//            // Cross axis positioning and sizing (AlignItems)
//            float containerCrossAxisSize = (Direction == FlexDirection.Row) ? innerDim.Height : innerDim.Width;

//            // Check if child's cross size is "auto" (not explicitly set by pixels or percent)
//            // For StyleDimension, if Pixels and Precent are both 0, it might be considered "auto" for stretch purposes.
//            bool isCrossSizeAuto = (Direction == FlexDirection.Row) ? (child.Height.Pixels == 0 && child.Height.Precent == 0) : (child.Width.Pixels == 0 && child.Width.Precent == 0);

//            if (AlignItems == AlignItems.Stretch && isCrossSizeAuto)
//            {
//                if (Direction == FlexDirection.Row)
//                {
//                    childFinalHeight = containerCrossAxisSize - child.MarginTop - child.MarginBottom;
//                    childFinalY = child.MarginTop;
//                }
//                else // Column
//                {
//                    childFinalWidth = containerCrossAxisSize - child.MarginLeft - child.MarginRight;
//                    childFinalX = child.MarginLeft;
//                }
//            }
//            else
//            {
//                float childOuterCrossDimValue = (Direction == FlexDirection.Row) ? childOuterDim.Height : childOuterDim.Width;
//                float childMarginPrimaryCross = (Direction == FlexDirection.Row) ? child.MarginTop : child.MarginLeft;
//                float childMarginSecondaryCross = (Direction == FlexDirection.Row) ? child.MarginBottom : child.MarginRight;

//                float currentCrossPos = 0;
//                switch (AlignItems)
//                {
//                    case AlignItems.FlexStart:
//                        currentCrossPos = childMarginPrimaryCross;
//                        break;
//                    case AlignItems.Center:
//                        currentCrossPos = (containerCrossAxisSize - childOuterCrossDimValue) / 2f + childMarginPrimaryCross;
//                        break;
//                    case AlignItems.FlexEnd:
//                        currentCrossPos = containerCrossAxisSize - childOuterCrossDimValue + childMarginPrimaryCross; // This is effectively (containerCrossAxisSize - (childActualCrossSize + childMarginSecondaryCross))
//                        break;
//                    default: // FlexStart
//                        currentCrossPos = childMarginPrimaryCross;
//                        break;
//                }
//                if (Direction == FlexDirection.Row)
//                    childFinalY = currentCrossPos;
//                else
//                    childFinalX = currentCrossPos;
//            }

//            // Update child's DependencyProperties for position and potentially size
//            // These are relative to the parent's inner coordinate space.
//            child.Left = childFinalX;
//            child._top.TypedValue = childFinalY;
//            if (AlignItems == AlignItems.Stretch && isCrossSizeAuto) // Only update if stretched
//            {
//                if (Direction == FlexDirection.Row)
//                    child._height.TypedValue = Math.Max(0, childFinalHeight); // Ensure non-negative
//                else
//                    child._width.TypedValue = Math.Max(0, childFinalWidth); // Ensure non-negative
//            }

//            // Advance main position for the next child
//            currentMainPos += childMainSize + actualGap;

//            // After the flex container has dictated the child's properties,
//            // the child needs to recalculate its own internal layout (dimensions, innerDimensions, outerDimensions)
//            // and then lay out its own children if it's a container.
//            child.Recalculate();
//        }
//    }
//}
