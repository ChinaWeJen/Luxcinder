using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReLogic.Graphics;
using Terraria.GameInput;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;
public class LuxUI
{
	private delegate void LuxMouseElementEvent(LuxcinderUIBase element, LuxUIMouseEvent evt);

	private class InputPointerCache
	{
		public double LastTimeDown;
		public bool WasDown;
		public LuxcinderUIBase LastDown;
		public LuxcinderUIBase LastClicked;
		public LuxMouseElementEvent MouseDownEvent;
		public LuxMouseElementEvent MouseUpEvent;
		public LuxMouseElementEvent ClickEvent;
		public LuxMouseElementEvent DoubleClickEvent;

		public void Clear()
		{
			LastClicked = null;
			LastDown = null;
			LastTimeDown = 0.0;
		}
	}

	private const double DOUBLE_CLICK_TIME = 500.0;
	private const double STATE_CHANGE_CLICK_DISABLE_TIME = 200.0;
	private const int MAX_HISTORY_SIZE = 32;
	private const int HISTORY_PRUNE_SIZE = 4;
	public static LuxUI ActiveInstance = new LuxUI();
	private List<LuxUIState> _history = new List<LuxUIState>();
	private InputPointerCache LeftMouse = new InputPointerCache
	{
		MouseDownEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.LeftMouseDown(evt);
		},
		MouseUpEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.LeftMouseUp(evt);
		},
		ClickEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.LeftClick(evt);
		},
		DoubleClickEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.LeftDoubleClick(evt);
		}
	};
	private InputPointerCache RightMouse = new InputPointerCache
	{
		MouseDownEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.RightMouseDown(evt);
		},
		MouseUpEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.RightMouseUp(evt);
		},
		ClickEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.RightClick(evt);
		},
		DoubleClickEvent = delegate (LuxcinderUIBase element, LuxUIMouseEvent evt) {
			element.RightDoubleClick(evt);
		}
	};
	public Vector2 MousePosition;
	private LuxcinderUIBase _lastElementHover;
	private double _clickDisabledTimeRemaining;
	private bool _isStateDirty;
	public bool IsVisible;
	private LuxUIState _currentState;

	public LuxUIState CurrentState => _currentState;

	public void ClearPointers()
	{
		LeftMouse.Clear();
		RightMouse.Clear();
	}

	public void ResetLasts()
	{
		if (_lastElementHover != null)
			_lastElementHover.MouseOut(new LuxUIMouseEvent(_lastElementHover, MousePosition));

		ClearPointers();
		_lastElementHover = null;
	}

	public void EscapeElements()
	{
		ResetLasts();
	}

	public LuxUI()
	{
		ActiveInstance = this;
	}

	public void Use()
	{
		if (ActiveInstance != this)
		{
			ActiveInstance = this;
			Recalculate();
		}
		else
		{
			ActiveInstance = this;
		}
	}

	private void ImmediatelyUpdateInputPointers()
	{
		LeftMouse.WasDown = Main.mouseLeft;
		RightMouse.WasDown = Main.mouseRight;
	}

	private void ResetState()
	{
		if (!Main.dedServ)
		{
			GetMousePosition();
			ImmediatelyUpdateInputPointers();
			if (_lastElementHover != null)
				_lastElementHover.MouseOut(new LuxUIMouseEvent(_lastElementHover, MousePosition));
		}

		ClearPointers();
		_lastElementHover = null;
		_clickDisabledTimeRemaining = Math.Max(_clickDisabledTimeRemaining, 200.0);
	}

	private void GetMousePosition()
	{
		MousePosition = new Vector2(Main.mouseX, Main.mouseY);
	}

	public void Update(GameTime time)
	{
		if (_currentState == null)
			return;

		GetMousePosition();
		LuxcinderUIBase uIElement = (Main.hasFocus ? _currentState.GetElementAt(MousePosition) : null);
		_clickDisabledTimeRemaining = Math.Max(0.0, _clickDisabledTimeRemaining - time.ElapsedGameTime.TotalMilliseconds);
		bool num = _clickDisabledTimeRemaining > 0.0;

		LayoutSolver solver = new LayoutSolver();
		solver.AddRoot(_currentState);
		solver.Solve();

		_currentState.Recalculate();

		try
		{
			Update_Inner(time, uIElement, ref num);
		}
		catch
		{
			throw;
		}
		finally
		{
			Update_End(time);
		}
	}

	// A split to add a try-catch-finally block without indentation issues.
	private void Update_Inner(GameTime time, LuxcinderUIBase uIElement, ref bool num)
	{
		if (uIElement != _lastElementHover)
		{
			if (_lastElementHover != null)
				_lastElementHover.MouseOut(new LuxUIMouseEvent(_lastElementHover, MousePosition));

			uIElement?.MouseOver(new LuxUIMouseEvent(uIElement, MousePosition));
			_lastElementHover = uIElement;
		}

		if (!num)
		{
			HandleClick(LeftMouse, time, Main.mouseLeft && Main.hasFocus, uIElement);
			HandleClick(RightMouse, time, Main.mouseRight && Main.hasFocus, uIElement);
		}

		if (PlayerInput.ScrollWheelDeltaForUI != 0)
		{
			uIElement?.ScrollWheel(new LuxUIScrollWheelEvent(uIElement, MousePosition, PlayerInput.ScrollWheelDeltaForUI));
			// Moved to after SystemHooks.UpdateUI(gameTime);
			/*
			PlayerInput.ScrollWheelDeltaForUI = 0;
			*/
		}
	}

	// Another split, to be called in 'finally'.
	private void Update_End(GameTime time)
	{
		if (_currentState != null)
			_currentState.Update(time);
	}


	private void HandleClick(InputPointerCache cache, GameTime time, bool isDown, LuxcinderUIBase mouseElement)
	{
		if (isDown && !cache.WasDown && mouseElement != null)
		{
			cache.LastDown = mouseElement;
			cache.MouseDownEvent(mouseElement, new LuxUIMouseEvent(mouseElement, MousePosition));
			if (cache.LastClicked == mouseElement && time.TotalGameTime.TotalMilliseconds - cache.LastTimeDown < 500.0)
			{
				cache.DoubleClickEvent(mouseElement, new LuxUIMouseEvent(mouseElement, MousePosition));
				cache.LastClicked = null;
			}

			cache.LastTimeDown = time.TotalGameTime.TotalMilliseconds;
		}
		else if (!isDown && cache.WasDown && cache.LastDown != null)
		{
			LuxcinderUIBase lastDown = cache.LastDown;
			if (lastDown.ContainsPoint(MousePosition))
			{
				cache.ClickEvent(lastDown, new LuxUIMouseEvent(lastDown, MousePosition));
				cache.LastClicked = cache.LastDown;
			}

			cache.MouseUpEvent(lastDown, new LuxUIMouseEvent(lastDown, MousePosition));
			cache.LastDown = null;
		}

		cache.WasDown = isDown;
	}

	public void Draw(SpriteBatch spriteBatch, GameTime time)
	{
		Use();
		if (_currentState != null)
		{
			if (_isStateDirty)
			{
				_currentState.Recalculate();
				_isStateDirty = false;
			}

			_currentState.Draw(spriteBatch);
		}
	}

	public void DrawDebugHitbox(BasicDebugDrawer drawer)
	{
		_ = _currentState;
	}

	public void SetState(LuxUIState state)
	{
		if (state == _currentState)
			return;

		if (state != null)
			AddToHistory(state);

		if (_currentState != null)
		{
			if (_lastElementHover != null)
				_lastElementHover.MouseOut(new LuxUIMouseEvent(_lastElementHover, MousePosition));

			_currentState.Deactivate();
		}

		_currentState = state;
		ResetState();
		if (state != null)
		{
			_isStateDirty = true;
			state.Activate();
			state.Recalculate();
		}
	}

	public void GoBack()
	{
		if (_history.Count >= 2)
		{
			LuxUIState state = _history[_history.Count - 2];
			_history.RemoveRange(_history.Count - 2, 2);
			SetState(state);
		}
	}

	private void AddToHistory(LuxUIState state)
	{
		_history.Add(state);
		if (_history.Count > 32)
			_history.RemoveRange(0, 4);
	}

	public void Recalculate()
	{
		if (_currentState != null)
			_currentState.Recalculate();
	}

	public CalculatedStyle GetDimensions()
	{
		Vector2 originalScreenSize = PlayerInput.OriginalScreenSize;
		return new CalculatedStyle(0f, 0f, originalScreenSize.X / Main.UIScale, originalScreenSize.Y / Main.UIScale);
	}

	internal void RefreshState()
	{
		if (_currentState != null)
			_currentState.Deactivate();

		ResetState();
		_currentState.Activate();
		_currentState.Recalculate();
	}

	public bool IsElementUnderMouse()
	{
		if (IsVisible && _lastElementHover != null)
			return !(_lastElementHover is LuxcinderUIBase);

		return false;
	}
}
