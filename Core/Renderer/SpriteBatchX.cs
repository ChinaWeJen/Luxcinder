using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Luxcinder.Core.Renderer;
public struct SpriteBatchXState
{
	public SpriteSortMode SpriteSortMode;
	public SamplerState SamplerState;
	public BlendState BlendState;
	public DepthStencilState DepthStencilState;
	public RasterizerState RasterizerState;
	public Effect Effect;
	public Matrix TransformMatrix;
	public Rectangle ScissorRectangle; 
}

/// <summary>
/// 增加栈式管理的SpriteBatch，允许在渲染过程中保存和恢复状态。
/// </summary>
public class SpriteBatchX
{
	private readonly SpriteBatch _wrappedSpriteBatch;
	private readonly GraphicsDevice _graphicsDevice;

	// Fields to store original states
	private List<SpriteBatchXState> _spriteBatchStates;

	public GraphicsDevice GraphicsDevice => _graphicsDevice;
	public SpriteBatch WrappedSpriteBatch => _wrappedSpriteBatch;

	private Matrix _lastTransformMatrix = Matrix.Identity;
	private Rectangle _lastScissorRectangle = Rectangle.Empty;

	// 辅助方法，用于通过反射获取私有字段的值
	private T GetPrivateFieldValue<T>(object obj, string fieldName)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));
		Type type = obj.GetType();
		FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
		if (field == null)
		{
			// 尝试查找基类中的字段，因为某些实现可能将字段放在基类中
			Type baseType = type.BaseType;
			while (baseType != null && field == null)
			{
				field = baseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
				baseType = baseType.BaseType;
			}
		}

		if (field == null)
			throw new InvalidOperationException($"Private field '{fieldName}' not found in type '{type.FullName}' or its base types. This may indicate an incompatible SpriteBatch version or implementation.");

		object value = field.GetValue(obj);
		if (value == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
		{
			// 如果字段值为null，但目标类型T是非可空值类型，则返回default(T)
			// 对于SpriteSortMode这样的枚举，这通常是第一个枚举值。
			// 对于Matrix，它会是Matrix.Identity，这通常是期望的。
			return default(T);
		}
		// 如果字段值为null，并且T是引用类型或可空值类型，则(T)value将正确处理。
		return (T)value;
	}

	public SpriteBatchX(SpriteBatch spriteBatch)
	{
		if (spriteBatch == null)
			throw new ArgumentNullException(nameof(spriteBatch));

		_wrappedSpriteBatch = spriteBatch;
		_graphicsDevice = _wrappedSpriteBatch.GraphicsDevice;

		// 用反射读取SpriteBatch的私有字段，存一份SpriteBatchXState
		_spriteBatchStates = new List<SpriteBatchXState>
			{
				new SpriteBatchXState
				{
                    // 注意：如果_wrappedSpriteBatch尚未调用Begin，这些字段可能包含默认值或null
                    SpriteSortMode = GetPrivateFieldValue<SpriteSortMode>(_wrappedSpriteBatch, "sortMode"),
					BlendState = GetPrivateFieldValue<BlendState>(_wrappedSpriteBatch, "blendState"),
					SamplerState = GetPrivateFieldValue<SamplerState>(_wrappedSpriteBatch, "samplerState"),
					DepthStencilState = GetPrivateFieldValue<DepthStencilState>(_wrappedSpriteBatch, "depthStencilState"),
					RasterizerState = GetPrivateFieldValue<RasterizerState>(_wrappedSpriteBatch, "rasterizerState"),
					Effect = GetPrivateFieldValue<Effect>(_wrappedSpriteBatch, "customEffect"),
					TransformMatrix = GetPrivateFieldValue<Matrix>(_wrappedSpriteBatch, "transformMatrix"),
                    ScissorRectangle = _graphicsDevice.ScissorRectangle // ScissorRectangle直接从GraphicsDevice获取
                }
			};
		_lastScissorRectangle = _graphicsDevice.ScissorRectangle;
		_lastTransformMatrix = GetPrivateFieldValue<Matrix>(_wrappedSpriteBatch, "transformMatrix");
	}

	private void RecordGraphicsStates(SpriteSortMode sortMode,
		BlendState blendState,
		SamplerState samplerState,
		DepthStencilState depthStencilState,
		RasterizerState rasterizerState,
		Effect effect,
		Matrix transformMatrix,
		Rectangle scissorRectangle)
	{
		SpriteBatchXState state = new SpriteBatchXState
		{
			SpriteSortMode = sortMode,
			BlendState = blendState,
			DepthStencilState = depthStencilState,
			RasterizerState = rasterizerState,
			SamplerState = samplerState,
			Effect = effect,
			TransformMatrix = transformMatrix,
			ScissorRectangle = scissorRectangle
		};
		_spriteBatchStates.Add(state);
	}

	private SpriteBatchXState RestoreGraphicsStates()
	{
		SpriteBatchXState state = _spriteBatchStates.Last();
		_spriteBatchStates.RemoveAt(_spriteBatchStates.Count - 1);
		return state;
	}

	public void Push(
		SpriteSortMode sortMode = SpriteSortMode.Deferred,
		BlendState blendState = null,
		SamplerState samplerState = null,
		DepthStencilState depthStencilState = null,
		RasterizerState rasterizerState = null,
		Effect effect = null,
		Matrix? transformMatrix = null,
		Rectangle? scissorRectangle = null)
	{
		_wrappedSpriteBatch.End();
		if (scissorRectangle.HasValue)
		{
			_wrappedSpriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle.Value;
			_lastScissorRectangle = scissorRectangle.Value;
		}
		else
		{
			scissorRectangle = _lastScissorRectangle;
		}
		if (transformMatrix.HasValue)
		{
			_lastTransformMatrix = transformMatrix.Value;
		}
		else
		{
			transformMatrix = _lastTransformMatrix;
		}
		RecordGraphicsStates(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix.Value, scissorRectangle.Value);

		_wrappedSpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix.Value);
	}

	// Simplified Begin for common Terraria UI usage
	public void Push(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Matrix transformMatrix)
	{
		Push(sortMode, blendState, samplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformMatrix);
	}


	public void Pop()
	{
		_wrappedSpriteBatch.End();
		if(_spriteBatchStates != null && _spriteBatchStates.Count != 0)
		{
			var state = RestoreGraphicsStates();

			_graphicsDevice.ScissorRectangle = state.ScissorRectangle;
			
			_wrappedSpriteBatch.Begin(
				state.SpriteSortMode,
				state.BlendState,
				state.SamplerState,
				state.DepthStencilState,
				state.RasterizerState,
				state.Effect,
				state.TransformMatrix);
		}
		else
		{
			// 报错
			throw new InvalidOperationException("SpriteBatchX: Cannot pop when there are no states to restore. Ensure you have called Push before Pop.");
		}
	}

	// --- Draw Overloads ---
	// (Mirroring common SpriteBatch.Draw overloads)

	public void Draw(Texture2D texture, Vector2 position, Color color)
	{
		_wrappedSpriteBatch.Draw(texture, position, color);
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
	{
		_wrappedSpriteBatch.Draw(texture, position, sourceRectangle, color);
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
	{
		_wrappedSpriteBatch.Draw(texture, destinationRectangle, color);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	{
		_wrappedSpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
	}

	// --- DrawString Overloads ---
	public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color);
	}

	public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	}

	public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	}

	public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color);
	}

	public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	}

	public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
	{
		_wrappedSpriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
	}
}