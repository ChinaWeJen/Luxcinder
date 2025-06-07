# NPC方向控制模板

## 核心功能
提供标准化的NPC面向方向控制方案，包括：
- 移动时方向控制
- 静止时面向玩家
- 动画同步
- 平滑转向

## 代码实现

```csharp
// 参数配置
float turnSpeed = 0.15f; // 转向速度(0-1)

// 方向控制主逻辑
Vector2 toPlayer = playerPos - NPC.Center;

if (NPC.velocity.X != 0) 
{
    // 移动方向定义(1=右, -1=左)
    int moveDirection = NPC.velocity.X > 0 ? -1 : 1;
    
    // 远离玩家时反转方向
    if ((NPC.velocity.X > 0 && toPlayer.X < 0) || 
        (NPC.velocity.X < 0 && toPlayer.X > 0)) 
    {
        moveDirection *= -1;
    }
    
    // 平滑转向
    if (NPC.spriteDirection != moveDirection && Main.rand.NextFloat() < turnSpeed) 
    {
        NPC.spriteDirection = moveDirection;
    }
}
else 
{
    // 静止时面向玩家
    NPC.spriteDirection = toPlayer.X > 0 ? -1 : 1;
}

// 动画方向同步
if (NPC.velocity.X * NPC.spriteDirection > 0) {
    currentFrame = frameCount - 1 - currentFrame;
}
```

## 使用说明

1. 将代码复制到NPC类的AI方法中
2. 确保已定义：
   - `playerPos` (玩家位置)
   - `currentFrame` (当前动画帧)
   - `frameCount` (动画总帧数)
3. 调整`turnSpeed`改变转向灵敏度

## 注意事项
- 1表示右，-1表示左（已反转标准值）
- 动画系统需要配合使用
- 测试各种移动场景下的表现