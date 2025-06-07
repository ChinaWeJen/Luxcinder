using System;
using Microsoft.Xna.Framework;
namespace Luxcinder.Core.BehaviorTree
{
    public class SecondOrderDynamics
    {
        // 状态变量
        private Vector2 _prevInput;
        private Vector2 _position;
        private Vector2? _velocity;

        // 动力学参数
        private float _k1, _k2, _k3;
        private float _criticalStep;

        public SecondOrderDynamics(float frequency, float dampingRatio, float responseScale)
        {
            SetConstants(frequency, dampingRatio, responseScale);
        }

        // 初始化状态
        public void Init(Vector2 x0, Vector2? v0 = null)
        {
            _prevInput = x0;
            _position = x0;
            _velocity ??= Vector2.Zero;
        }

        // 用于设置系统参量
        public void SetConstants(float f, float z, float r)
        {
            _k1 = z / (MathF.PI * f);
            _k2 = 1 / (MathF.Pow(2 * MathF.PI * f, 2));
            _k3 = r * z / (2 * MathF.PI * f);

            // 计算稳定临界步长
            _criticalStep = 0.8f * (MathF.Sqrt(4 * _k2 + _k1 * _k1) - _k1);
        }

        public Vector2 Update(float delta, Vector2 targetPosition, Vector2? targetVelocity = null)
        {
            // 估算速度
            Vector2 xd = targetVelocity ?? (targetPosition - _prevInput) / delta;
            _prevInput = targetPosition;

            // 分步计算防止超调
            int iterations = (int)Math.Ceiling(delta / _criticalStep);
            delta = delta / iterations;

            for (int i = 0; i < iterations; i++)
            {
                // 位置积分
                _position += delta * _velocity.Value;
                // 速度积分（基于加速度）
                _velocity += delta * ((targetPosition + _k3 * xd - _position - _k1 * _velocity) / _k2);
            }

            return _position;
        }
    }
}
