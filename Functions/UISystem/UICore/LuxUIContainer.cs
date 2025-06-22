using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Luxcinder.Core.Renderer;
using ReLogic.Graphics;
using Spine;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;


public enum TextLayout
{
	Inline,
	Block,
	AutoWrap
}

/// <summary>
/// Represents a dimension, either an absolute pixel size, a percentage of the available space, or a combination of both.
/// <para/> For example <c>uiElement.Width.Set(200, 0f);</c> sets an absolute width of 200 pixels. <c>uiElement.Width.Set(0, 0.5f);</c> on the otherhand sets a width of 50% of the parent's avaiable <see cref="UIElement.GetInnerDimensions"/>.
/// <para/> Both values can be set for more complex logic. <c>uiElement.Width.Set(-10, 0.5f);</c> sets the width to 50% of the available space minus 10 pixels. This would leave room between 2 buttons filling a space. <c>uiElement.Height.Set(-100, 1f);</c> would fill the full height of the space but leave 100 pixels at the bottom empty.
/// </summary>
public struct StyleDimension
{
	public static StyleDimension Fill_Inf = new StyleDimension(float.PositiveInfinity, 0);
	public static StyleDimension Fill = new StyleDimension(0f, 1);
    public static StyleDimension Empty = new StyleDimension(0f, 0f);
    public float Pixels;
    public float Precent;
	public bool AutoGrow = false;

    // Added by TML.
    public float Percent
    {
        get => Precent;
        set => Precent = value;
	}

	public StyleDimension(float pixels, float precent)
	{
		Pixels = pixels;
		Precent = precent;
	}

	public void SetAuto(bool autoGrow)
	{
		AutoGrow = autoGrow;
	}

	/// <summary>
	/// Sets the values for this StyleDimension.
	/// <para/> <b>StyleDimension documentation:</b><br/><inheritdoc cref="StyleDimension"/>
	/// </summary>
	/// <param name="pixels"></param>
	/// <param name="precent"></param>
	public void Set(float pixels, float precent)
    {
        Pixels = pixels;
        Precent = precent;
    }

    public float GetValue(float containerSize) => Pixels + Precent * containerSize;
    public static StyleDimension FromPixels(float pixels) => new StyleDimension(pixels, 0f);
    public static StyleDimension FromPercent(float percent) => new StyleDimension(0f, percent);
    public static StyleDimension FromPixelsAndPercent(float pixels, float percent) => new StyleDimension(pixels, percent);
}

public class LuxUIContainer
{
	public delegate void MouseEvent(LuxUIMouseEvent evt, LuxUIContainer listeningElement);

	public delegate void ScrollWheelEvent(LuxUIScrollWheelEvent evt, LuxUIContainer listeningElement);

	/// <summary> How far down from the top edge of the <see cref="Parent"/> element's <see cref="GetInnerDimensions"/> that this element will be positioned. See also <see cref="HAlign"/> for another positioning option. </summary>
	public StyleDimension Top;
	/// <summary> How far right from the left edge of the <see cref="Parent"/> element's <see cref="GetInnerDimensions"/> that this element will be positioned. See also <see cref="VAlign"/> for another positioning option. </summary>
	public StyleDimension Left;
	/// <summary> How wide this element intends to be. The calculated width will be clamped between <see cref="MinWidth"/> and <see cref="MaxWidth"/> according to the <see cref="GetInnerDimensions"/> of the parent element. </summary>
	public StyleDimension Width;
	/// <summary> How tall this element intends to be. The calculated height will be clamped between <see cref="MinHeight"/> and <see cref="MaxHeight"/> according to the <see cref="GetInnerDimensions"/> of the parent element. </summary>
	public StyleDimension Height;

	/// <summary> The minimum width of this element. Defaults to no width. </summary>
	public StyleDimension MinWidth = StyleDimension.Empty;
	/// <summary> The maximum width of this element. Defaults to no height. </summary>
	public StyleDimension MinHeight = StyleDimension.Empty;
	/// <summary> The maximum width of this element. Defaults to the full width. </summary>
	public StyleDimension MaxWidth = StyleDimension.Fill_Inf;
	/// <summary> The maximum height of this element. Defaults to the full height. </summary>
	public StyleDimension MaxHeight = StyleDimension.Fill_Inf;

	/// <summary>
	/// The dimensions of the area covered by this element. This is the area of this element interactible by the mouse.
	/// <para/> The width and height are derived from the <see cref="Width"/> and <see cref="Height"/> values of this element and will be limited by <see cref="MinWidth"/>/MaxWidth/MinHeight/MaxHeight as well as the <see cref="GetInnerDimensions"/> of the parent element.
	/// <para/> The position is derived from the <see cref="Top"/>, <see cref="Left"/>, <see cref="HAlign"/>, <see cref="VAlign"/>, and <see cref="MarginLeft"/>/Right/Top/Bottom values of this element as well as the <see cref="GetInnerDimensions"/> of the parent element.
	/// </summary>
	public CalculatedStyle GetDimensions()
	{
		return new CalculatedStyle(ResolvedLeft, ResolvedTop, ResolvedWidth, ResolvedHeight);
	}

	public CalculatedStyle GetInnerDimensions()
	{
		return new CalculatedStyle(ResolvedInnerLeft, ResolvedInnerTop, ResolvedInnerWidth, ResolvedInnerHeight);
	}

	public CalculatedStyle GetOuterDimensions()
	{
		return new CalculatedStyle(ResolvedOuterLeft, ResolvedOuterTop, ResolvedOuterWidth, ResolvedOuterHeight);
	}

	// 核心布局属性
	protected DependencyProperty<float> _top = new();
	protected DependencyProperty<float> _left = new();
	protected DependencyProperty<float> _width = new();
	protected DependencyProperty<float> _height = new();

	public float ResolvedTop => ResolvedOuterTop + MarginTop;
	public float ResolvedLeft => ResolvedOuterLeft + MarginLeft;
	public float ResolvedWidth => ResolvedOuterWidth - MarginLeft - MarginRight;
	public float ResolvedHeight => ResolvedOuterHeight - MarginTop - MarginBottom;

	public float ResolvedInnerTop => ResolvedTop + PaddingTop;
	public float ResolvedInnerLeft => ResolvedLeft + PaddingLeft;
	public float ResolvedInnerWidth => ResolvedWidth - PaddingLeft - PaddingRight;
	public float ResolvedInnerHeight => ResolvedHeight - PaddingTop - PaddingBottom;

	public float ResolvedOuterTop => _top.TypedValue;
	public float ResolvedOuterLeft => _left.TypedValue;
	public float ResolvedOuterWidth => _width.TypedValue;
	public float ResolvedOuterHeight => _height.TypedValue;

	public float MarginLeft = 0;
	public float MarginRight = 0;
	public float MarginTop = 0;
	public float MarginBottom = 0;

	public float PaddingLeft = 0;
	public float PaddingRight = 0;
	public float PaddingTop = 0;
	public float PaddingBottom = 0;

	public event MouseEvent OnLeftMouseDown;
	public event MouseEvent OnLeftMouseUp;
	public event MouseEvent OnLeftClick;
	public event MouseEvent OnLeftDoubleClick;
	public event MouseEvent OnRightMouseDown;
	public event MouseEvent OnRightMouseUp;
	public event MouseEvent OnRightClick;
	public event MouseEvent OnRightDoubleClick;
	/// <summary> Called by <see cref="MouseOver(UIMouseEvent)"/>. Use this event instead of inheritance if suitable. </summary>
	public event MouseEvent OnMouseOver;
	/// <summary> Called by <see cref="MouseOut(UIMouseEvent)"/>. Use this event instead of inheritance if suitable. </summary>
	public event MouseEvent OnMouseOut;
	public event ScrollWheelEvent OnScrollWheel;

	public bool IsMouseHovering { get; private set; }

	public bool Visible
	{
		get; set;
	} = true;

	public bool OverflowHidden
	{
		get;
		set;
	} = false;

	private bool _isInitialized;

	// 子元素
	public List<LuxUIContainer> Children { get; } = new();

	private static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
	{
		CullMode = CullMode.None,
		ScissorTestEnable = true
	};

	// 父节点
	public LuxUIContainer Parent
	{
		get; set;
	}
	public bool IgnoresMouseInteraction { get; set; }


	protected virtual float ResolveTop(CalculatedStyle topMostDimensions)
	{
		float height = topMostDimensions.Height;
		if (Parent != null)
		{
			if (Top.Percent > 0f)
			{
				height = Parent.ResolvedInnerHeight;
			}
			topMostDimensions.Y = Parent.ResolvedInnerTop;
		}
		return topMostDimensions.Y + Top.Pixels + Top.Percent * height;
	}

	protected virtual float ResolveLeft(CalculatedStyle topMostDimensions)
	{
		float width = topMostDimensions.Width;
		if (Parent != null)
		{
			if (Left.Percent > 0f)
			{
				width = Parent.ResolvedInnerWidth;
			}
			topMostDimensions.X = Parent.ResolvedInnerLeft;
		}
		return topMostDimensions.X + Left.Pixels + Left.Percent * width;
	}

	protected virtual float ResolveWidth(CalculatedStyle topMostDimensions)
	{
		float width = topMostDimensions.Width;
		if (Parent != null)
		{
			if (Width.Percent > 0f)
			{
				width = Parent.ResolvedInnerWidth;
			}
		}
		float maxWidth = MaxWidth.GetValue(width);
		float minWidth = MinWidth.GetValue(width);
		if (Width.AutoGrow)
		{
			width = 0;
			foreach (var child in Children)
			{
				width += child.ResolvedOuterWidth;
			}
			return MathHelper.Clamp(width + PaddingLeft + PaddingRight, minWidth, maxWidth);
		}
		else
		{
			return MathHelper.Clamp(Width.Pixels + Width.Percent * width, minWidth, maxWidth);
		}
	}

	protected virtual float ResolveHeight(CalculatedStyle topMostDimensions)
	{
		float height = topMostDimensions.Height;
		if (Parent != null)
		{
			if (Height.Percent > 0f)
			{
				height = Parent.ResolvedInnerHeight;
			}
		}
		float maxHeight = MaxHeight.GetValue(height);
		float minHeight = MinHeight.GetValue(height);
		if (Height.AutoGrow)
		{
			height = 0;
			foreach (var child in Children)
			{
				height += child.ResolvedOuterHeight;
			}
			return MathHelper.Clamp(height + PaddingTop + PaddingBottom, minHeight, maxHeight);
		}
		else
		{
			return MathHelper.Clamp(Height.Pixels + Height.Percent * height, minHeight, maxHeight);
		}
	}

	public void SetVisibleRecursive(bool visible)
	{
		Visible = visible;
		foreach (var child in Children)
		{
			child.SetVisibleRecursive(visible);
		}
	}

	// 初始化依赖关系
	public virtual void InitializeDependencies()
	{
		// 默认情况下，宽高不依赖其他属性
		// 子类可以重写此方法来建立依赖关系
		CalculatedStyle topMostDimensions = LuxUI.ActiveInstance.GetDimensions();
		_top.Bind(() =>
		{
			return ResolveTop(topMostDimensions);
		});
		_left.Bind(() =>
		{
			return ResolveLeft(topMostDimensions);
		});
		_width.Bind(() =>
		{
			return ResolveWidth(topMostDimensions);
		});
		_height.Bind(() =>
		{
			return ResolveHeight(topMostDimensions);
		});

		foreach (var child in Children)
		{
			child.InitializeDependencies();
		}
	}

	public void ResetDependencyStates()
	{
		_top.ResetDependencyState();
		_left.ResetDependencyState();
		_width.ResetDependencyState();
		_height.ResetDependencyState();

		foreach (var child in Children)
		{
			child.ResetDependencyStates();
		}
	}

	public virtual void ResolveDependencies()
	{
		_top.Resolve();
		_left.Resolve();
		_width.Resolve();
		_height.Resolve();

		foreach (var child in Children)
		{
			child.ResolveDependencies();
		}
	}

	public void Recalculate()
	{
		//      CalculatedStyle parentDimensions = Parent == null ? LuxUI.ActiveInstance.GetDimensions() : Parent.GetDimensions();

		//      CalculatedStyle result = default;
		//result.X = _left.TypedValue + parentDimensions.X;
		//result.Y = _top.TypedValue + parentDimensions.Y;
		//result.Width = _width.TypedValue;
		//result.Height = _height.TypedValue;

		//_dimensions = result;

		// 更新子元素的布局
		RecalculateChildren();
	}

	public virtual void RecalculateChildren()
	{
		foreach (var child in Children)
		{
			child.Recalculate();
		}
	}

	// 添加子节点
	public void AddChild(LuxUIContainer child)
	{
		child.Parent = this;
		Children.Add(child);
	}

	[Conditional("DEBUG")]
	public void DrawDebugHitbox(BasicDebugDrawer drawer, bool drawChildren)
	{
		_drawDebugHitbox(drawer, drawChildren, 0);
	}

	private void _drawDebugHitbox(BasicDebugDrawer drawer, bool drawChildren, int level)
	{
		Color color = Color.Yellow * 0.66f;
		if (level % 3 == 1)
		{
			color = Color.Cyan * 0.66f;
		}
		else if (level % 3 == 2)
		{
			color = Color.Pink * 0.66f;
		}

		CalculatedStyle dimensions = GetDimensions();
		drawer.DrawLine(dimensions.Position(), dimensions.Position() + new Vector2(dimensions.Width, 0f), 1f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(dimensions.Width, 0f), dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height), 1f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height), dimensions.Position() + new Vector2(0f, dimensions.Height), 1f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(0f, dimensions.Height), dimensions.Position(), 1f, color);

		CalculatedStyle innerDimensions = GetInnerDimensions();
		drawer.DrawLine(innerDimensions.Position(), innerDimensions.Position() + new Vector2(innerDimensions.Width, 0f), 1f, color);
		drawer.DrawLine(innerDimensions.Position() + new Vector2(innerDimensions.Width, 0f), innerDimensions.Position() + new Vector2(innerDimensions.Width, innerDimensions.Height), 1f, color);
		drawer.DrawLine(innerDimensions.Position() + new Vector2(innerDimensions.Width, innerDimensions.Height), innerDimensions.Position() + new Vector2(0f, innerDimensions.Height), 1f, color);
		drawer.DrawLine(innerDimensions.Position() + new Vector2(0f, innerDimensions.Height), innerDimensions.Position(), 1f, color);

		if (drawChildren)
		{
			foreach (var child in Children)
			{
				child._drawDebugHitbox(drawer, drawChildren, level + 1);
			}
		}
	}

	/// <summary>
	/// Called when the UIElement under the mouse is left clicked. The default code calls the <see cref="OnLeftMouseDown"/> event and then calls <see cref="LeftMouseDown"/> on the <see cref="Parent"/> element.
	/// <para/> Since the method is called on all parent elements in the hierarchy, check <c>if (evt.Target == this)</c> for code only interested in direct clicks to this element. Children elements overlaying this element can be ignored by setting <see cref="IgnoresMouseInteraction"/> to true on them.
	/// </summary>
	/// <param name="evt"></param>
	public virtual void LeftMouseDown(LuxUIMouseEvent evt)
	{
		if (this.OnLeftMouseDown != null)
			this.OnLeftMouseDown(evt, this);

		if (Parent != null)
			Parent.LeftMouseDown(evt);
	}

	public virtual void LeftMouseUp(LuxUIMouseEvent evt)
	{
		if (this.OnLeftMouseUp != null)
			this.OnLeftMouseUp(evt, this);

		if (Parent != null)
			Parent.LeftMouseUp(evt);
	}

	public virtual void LeftClick(LuxUIMouseEvent evt)
	{
		if (this.OnLeftClick != null)
			this.OnLeftClick(evt, this);

		if (Parent != null)
			Parent.LeftClick(evt);
	}

	public virtual void LeftDoubleClick(LuxUIMouseEvent evt)
	{
		if (this.OnLeftDoubleClick != null)
			this.OnLeftDoubleClick(evt, this);

		if (Parent != null)
			Parent.LeftDoubleClick(evt);
	}

	public virtual void RightMouseDown(LuxUIMouseEvent evt)
	{
		if (this.OnRightMouseDown != null)
			this.OnRightMouseDown(evt, this);

		if (Parent != null)
			Parent.RightMouseDown(evt);
	}

	public virtual void RightMouseUp(LuxUIMouseEvent evt)
	{
		if (this.OnRightMouseUp != null)
			this.OnRightMouseUp(evt, this);

		if (Parent != null)
			Parent.RightMouseUp(evt);
	}

	public virtual void RightClick(LuxUIMouseEvent evt)
	{
		if (this.OnRightClick != null)
			this.OnRightClick(evt, this);

		if (Parent != null)
			Parent.RightClick(evt);
	}

	public virtual void RightDoubleClick(LuxUIMouseEvent evt)
	{
		if (this.OnRightDoubleClick != null)
			this.OnRightDoubleClick(evt, this);

		if (Parent != null)
			Parent.RightDoubleClick(evt);
	}

	public virtual void MouseOver(LuxUIMouseEvent evt)
	{
		IsMouseHovering = true;
		if (this.OnMouseOver != null)
			this.OnMouseOver(evt, this);

		if (Parent != null)
			Parent.MouseOver(evt);
	}

	/// <summary>
	/// Called once when this UIElement is no longer moused over. Default implementation sets <see cref="IsMouseHovering"/> to false, calls <see cref="OnMouseOut"/> event, then calls this same method on the <see cref="Parent"/> element.
	/// <para/> Useful for changing visuals to indicate the element is no longer interactable, as is the <see cref="OnMouseOut"/> event.
	/// <para/> <see cref="MouseOver(UIMouseEvent)"/> will be called when it is hovered once again.
	/// </summary>
	public virtual void MouseOut(LuxUIMouseEvent evt)
	{
		IsMouseHovering = false;
		if (this.OnMouseOut != null)
			this.OnMouseOut(evt, this);

		if (Parent != null)
			Parent.MouseOut(evt);
	}

	public virtual void ScrollWheel(LuxUIScrollWheelEvent evt)
	{
		if (this.OnScrollWheel != null)
			this.OnScrollWheel(evt, this);

		if (Parent != null)
			Parent.ScrollWheel(evt);
	}

	public LuxUIContainer GetElementAt(Vector2 point)
	{
		LuxUIContainer uIElement = null;
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			LuxUIContainer uIElement2 = Children[num];
			if (!uIElement2.IgnoresMouseInteraction && uIElement2.ContainsPoint(point))
			{
				uIElement = uIElement2;
				break;
			}
		}

		if (uIElement != null)
			return uIElement.GetElementAt(point);

		if (IgnoresMouseInteraction)
			return null;

		if (ContainsPoint(point))
		{
			if (this is not LuxUIState)
			{
				return this;
			}
			return this;
		}

		return null;
	}

	public virtual bool ContainsPoint(Vector2 point)
	{
		var dimensions = GetDimensions();
		if (point.X > dimensions.X && point.Y > dimensions.Y && point.X < dimensions.X + dimensions.Width)
			return point.Y < dimensions.Y + dimensions.Height;
		return false;
	}

	public virtual void Update(GameTime gameTime)
	{
		foreach (LuxUIContainer element in Children)
		{
			element.Update(gameTime);
		}
	}

	public virtual void Draw(SpriteBatchX spriteBatch)
	{
		bool overflowHidden = OverflowHidden;
		bool useImmediateMode = false;
		SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
		if (Visible)
		{
			if (useImmediateMode)
			{
				spriteBatch.Push(SpriteSortMode.Immediate, BlendState.NonPremultiplied, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
				DrawSelf(spriteBatch);
				spriteBatch.Pop();
			}
			else
			{
				DrawSelf(spriteBatch);
			}
		}

		if (overflowHidden)
		{
			Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);
			Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.Push(SpriteSortMode.Deferred, BlendState.NonPremultiplied, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix, adjustedClippingRectangle);
			DrawChildren(spriteBatch);
			spriteBatch.Pop();
		}
		else
		{
			DrawChildren(spriteBatch);
		}
	}


	protected virtual void DrawSelf(SpriteBatchX spriteBatch)
	{
	}

	protected virtual void DrawChildren(SpriteBatchX spriteBatch)
	{
		foreach (LuxUIContainer element in Children)
		{
			element.Draw(spriteBatch);
		}
	}

	public Rectangle GetClippingRectangle(SpriteBatchX spriteBatch)
	{
		var dimensions = GetDimensions();
		Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
		Vector2 position = new Vector2(dimensions.Width, dimensions.Height) + vector;
		vector = Vector2.Transform(vector, Main.UIScaleMatrix);
		position = Vector2.Transform(position, Main.UIScaleMatrix);
		Rectangle rectangle = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
		int num = (int)((float)Main.screenWidth * Main.UIScale);
		int num2 = (int)((float)Main.screenHeight * Main.UIScale);
		rectangle.X = Terraria.Utils.Clamp(rectangle.X, 0, num);
		rectangle.Y = Terraria.Utils.Clamp(rectangle.Y, 0, num2);
		rectangle.Width = Terraria.Utils.Clamp(rectangle.Width, 0, num - rectangle.X);
		rectangle.Height = Terraria.Utils.Clamp(rectangle.Height, 0, num2 - rectangle.Y);
		Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
		int num3 = Terraria.Utils.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
		int num4 = Terraria.Utils.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
		int num5 = Terraria.Utils.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
		int num6 = Terraria.Utils.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);
		return new Rectangle(num3, num4, num5 - num3, num6 - num4);
	}

	public void SetPadding(float padding)
	{
		PaddingBottom = padding;
		PaddingLeft = padding;
		PaddingRight = padding;
		PaddingTop = padding;
	}

	public void SetMargin(float margin)
	{
		MarginBottom = margin;
		MarginLeft = margin;
		MarginRight = margin;
		MarginTop = margin;
	}

	public void Initialize()
	{
		OnInitialize();
		_isInitialized = true;
	}

	/// <summary>
	/// Called before the first time this element is activated (see <see cref="OnActivate"/>). Use this method to create and append other UIElement to this to build a UI. 
	/// </summary>
	public virtual void OnInitialize()
	{
	}


	public void Activate()
	{
		if (!_isInitialized)
			Initialize();

		OnActivate();
		foreach (LuxUIContainer element in Children)
		{
			element.Activate();
		}
	}

	/// <summary>
	/// Called each time this element is activated, which is usually when a <see cref="UIState"/> is activated via <see cref="UserInterface.SetState(UIState)"/>. Use this to run code to update elements whenever the UI is toggled on.
	/// </summary>
	public virtual void OnActivate()
	{
	}
	public void Deactivate()
	{
		OnDeactivate();
		foreach (LuxUIContainer element in Children)
		{
			element.Deactivate();
		}
	}

	public virtual void OnDeactivate()
	{
	}
}

