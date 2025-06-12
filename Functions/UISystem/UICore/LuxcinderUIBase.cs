using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ReLogic.Graphics;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;


/// <summary>
/// Represents a dimension, either an absolute pixel size, a percentage of the available space, or a combination of both.
/// <para/> For example <c>uiElement.Width.Set(200, 0f);</c> sets an absolute width of 200 pixels. <c>uiElement.Width.Set(0, 0.5f);</c> on the otherhand sets a width of 50% of the parent's avaiable <see cref="UIElement.GetInnerDimensions"/>.
/// <para/> Both values can be set for more complex logic. <c>uiElement.Width.Set(-10, 0.5f);</c> sets the width to 50% of the available space minus 10 pixels. This would leave room between 2 buttons filling a space. <c>uiElement.Height.Set(-100, 1f);</c> would fill the full height of the space but leave 100 pixels at the bottom empty.
/// </summary>
public struct StyleDimension
{
    public static StyleDimension Fill = new StyleDimension(0f, 1f);
    public static StyleDimension Empty = new StyleDimension(0f, 0f);
    public float Pixels;
    public float Precent;

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

public class LuxcinderUIBase
{
	public delegate void MouseEvent(LuxUIMouseEvent evt, LuxcinderUIBase listeningElement);

	public delegate void ScrollWheelEvent(LuxUIScrollWheelEvent evt, LuxcinderUIBase listeningElement);

	/// <summary> How far down from the top edge of the <see cref="Parent"/> element's <see cref="GetInnerDimensions"/> that this element will be positioned. See also <see cref="HAlign"/> for another positioning option. </summary>
	public StyleDimension Top;
	/// <summary> How far right from the left edge of the <see cref="Parent"/> element's <see cref="GetInnerDimensions"/> that this element will be positioned. See also <see cref="VAlign"/> for another positioning option. </summary>
	public StyleDimension Left;
	/// <summary> How wide this element intends to be. The calculated width will be clamped between <see cref="MinWidth"/> and <see cref="MaxWidth"/> according to the <see cref="GetInnerDimensions"/> of the parent element. </summary>
	public StyleDimension Width;
	/// <summary> How tall this element intends to be. The calculated height will be clamped between <see cref="MinHeight"/> and <see cref="MaxHeight"/> according to the <see cref="GetInnerDimensions"/> of the parent element. </summary>
	public StyleDimension Height;

	private CalculatedStyle _dimensions;

	/// <summary>
	/// The dimensions of the area covered by this element. This is the area of this element interactible by the mouse.
	/// <para/> The width and height are derived from the <see cref="Width"/> and <see cref="Height"/> values of this element and will be limited by <see cref="MinWidth"/>/MaxWidth/MinHeight/MaxHeight as well as the <see cref="GetInnerDimensions"/> of the parent element.
	/// <para/> The position is derived from the <see cref="Top"/>, <see cref="Left"/>, <see cref="HAlign"/>, <see cref="VAlign"/>, and <see cref="MarginLeft"/>/Right/Top/Bottom values of this element as well as the <see cref="GetInnerDimensions"/> of the parent element.
	/// </summary>
	public CalculatedStyle GetDimensions() => _dimensions;

	// 核心布局属性
	protected DependencyProperty<float> _top = new();
    protected DependencyProperty<float> _left = new();
    protected DependencyProperty<float> _width  = new();
    protected DependencyProperty<float> _height = new();

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

	public bool OverflowHidden;
	private bool _isInitialized;

	// 子元素
	public List<LuxcinderUIBase> Children { get; } = new();

	private static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
	{
		CullMode = CullMode.None,
		ScissorTestEnable = true
	};

	// 父节点
	public LuxcinderUIBase Parent
	{
		get; set;
	}
	public bool IgnoresMouseInteraction { get; set; }


	protected virtual float BindTop(CalculatedStyle topMostDimensions)
	{
        float height = topMostDimensions.Height;
        if (Parent != null)
        {
            if (Top.Percent > 0f)
            {
                height = Parent._height.TypedValue;
            }
        }
        return Top.Pixels + Top.Percent * height;
    }

    protected virtual float BindLeft(CalculatedStyle topMostDimensions)
    {
        float width = topMostDimensions.Width;
        if (Parent != null)
        {
            if (Left.Percent > 0f)
            {
                width = Parent._width.TypedValue;
            }
        }
        return Left.Pixels + Left.Percent * width;
    }

    protected virtual float BindWidth(CalculatedStyle topMostDimensions)
    {
        float width = topMostDimensions.Width;
        if (Parent != null)
        {
            if (Width.Percent > 0f)
            {
                width = Parent._width.TypedValue;
            }
        }
        return Width.Pixels + Width.Percent * width;
    }

    protected virtual float BindHeight(CalculatedStyle topMostDimensions)
    {	
		float height = topMostDimensions.Height;
        if (Parent != null)
        {
            if (Height.Percent > 0f)
            {
                height = Parent._height.TypedValue;
            }
        }
        return Height.Pixels + Height.Percent * height;
    }

    // 初始化依赖关系
    public virtual void InitializeDependencies()
	{
        // 默认情况下，宽高不依赖其他属性
        // 子类可以重写此方法来建立依赖关系
        CalculatedStyle topMostDimensions = LuxUI.ActiveInstance.GetDimensions();
        _top.Bind(() =>
		{
			return BindTop(topMostDimensions);
        });
        _left.Bind(() =>
        {
            return BindTop(topMostDimensions);
        });
        _width.Bind(() =>
		{
			return BindWidth(topMostDimensions);
		});
		_height.Bind(() =>
		{
			return BindHeight(topMostDimensions);
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
        CalculatedStyle parentDimensions = Parent == null ? LuxUI.ActiveInstance.GetDimensions() : Parent.GetDimensions();

        CalculatedStyle result = default;
		result.X = _left.TypedValue + parentDimensions.X;
		result.Y = _top.TypedValue + parentDimensions.Y;
		result.Width = _width.TypedValue;
		result.Height = _height.TypedValue;

		_dimensions = result;

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
	public void AddChild(LuxcinderUIBase child)
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
		Color color = Color.White;
		if (level % 3 == 1)
		{
			color = Color.Yellow;
		}
		else if(level % 3 == 2)
		{
			color = Color.Lime;
        }
		CalculatedStyle dimensions = GetDimensions();
		drawer.DrawLine(dimensions.Position(), dimensions.Position() + new Vector2(dimensions.Width, 0f), 2f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(dimensions.Width, 0f), dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height), 2f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height), dimensions.Position() + new Vector2(0f, dimensions.Height), 2f, color);
		drawer.DrawLine(dimensions.Position() + new Vector2(0f, dimensions.Height), dimensions.Position(), 2f, color);


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

	public LuxcinderUIBase GetElementAt(Vector2 point)
	{
		LuxcinderUIBase uIElement = null;
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			LuxcinderUIBase uIElement2 = Children[num];
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
			return this;

		return null;
	}

	public virtual bool ContainsPoint(Vector2 point)
	{
		if (point.X > _dimensions.X && point.Y > _dimensions.Y && point.X < _dimensions.X + _dimensions.Width)
			return point.Y < _dimensions.Y + _dimensions.Height;

		return false;
	}

	public virtual void Update(GameTime gameTime)
	{
		foreach (LuxcinderUIBase element in Children)
		{
			element.Update(gameTime);
		}
	}

	public virtual void Draw(SpriteBatch spriteBatch)
	{
		bool overflowHidden = OverflowHidden;
		bool useImmediateMode = false;
		RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
		Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
		SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
		if (useImmediateMode)
		{
			spriteBatch.End();
			spriteBatch.Begin(useImmediateMode ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
			DrawSelf(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}
		else
		{
			DrawSelf(spriteBatch);
		}

		if (overflowHidden)
		{
			spriteBatch.End();
			Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);

			/*
			spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;
			*/
			Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;

			spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}

		DrawChildren(spriteBatch);
		if (overflowHidden)
		{
			// TML: save a new rasterizer state snapshot to restore
			rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

			spriteBatch.End();
			spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
			spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
		}
	}


	protected virtual void DrawSelf(SpriteBatch spriteBatch)
	{
	}

	protected virtual void DrawChildren(SpriteBatch spriteBatch)
	{
		foreach (LuxcinderUIBase element in Children)
		{
			element.Draw(spriteBatch);
		}
	}

	public Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
	{
		Vector2 vector = new Vector2(_dimensions.X, _dimensions.Y);
		Vector2 position = new Vector2(_dimensions.Width, _dimensions.Height) + vector;
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
		foreach (LuxcinderUIBase element in Children)
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
		foreach (LuxcinderUIBase element in Children)
		{
			element.Deactivate();
		}
	}

	public virtual void OnDeactivate()
	{
	}
}
