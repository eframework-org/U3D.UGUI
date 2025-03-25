# UISpriteAnimation

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UISpriteAnimation 是一个UI精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。

## 功能特性

- 支持两种加载模式：图集模式和精灵模式
- 支持按前缀筛选图集中的精灵
- 可自定义动画帧率和循环播放设置
- 支持在编辑器模式下预览动画效果

## 使用手册

### 组件添加

1. 添加组件：
   - 选择UI游戏对象（必须有Image组件）
   - 在Inspector窗口点击"Add Component"
   - 选择"UI/Sprite Animation"或搜索"UISpriteAnimation"

### 精灵模式选择

1. 图集模式(Atlas)：
   - 从UIAtlas组件加载精灵
   - 确保游戏对象上有UIAtlas组件
   - 设置Prefix(前缀)筛选图集中的精灵
   - 匹配前缀的精灵将按名称排序组成动画序列

2. 精灵模式(Sprite)：
   - 直接使用精灵数组
   - 将精灵数组赋值给Sprites属性
   - 精灵将按数组顺序播放

### 动画设置

1. 基本设置：
   ```csharp
   // 设置动画帧率（每秒帧数）
   spriteAnimation.FrameRate = 30;
   
   // 设置是否循环播放
   spriteAnimation.Loop = true;
   
   // 设置精灵数组（精灵模式下）
   spriteAnimation.Sprites = mySprites;
   
   // 设置精灵名称前缀（图集模式下）
   spriteAnimation.Prefix = "run_";
   ```

2. 运行时控制：
   ```csharp
   // 重置动画到第一帧并重新开始播放
   spriteAnimation.Reset();
   
   // 检查动画是否处于活动状态
   if (spriteAnimation.Active) {
       Debug.Log("动画正在播放");
   }
   
   // 获取动画总帧数
   int totalFrames = spriteAnimation.Frames;
   ```

### 注意事项

1. 图集模式要点：
   - 精灵命名应有明确规律，便于按前缀筛选
   - 精灵将按名称字母顺序排序，可考虑使用数字前缀如"run_01"、"run_02"确保顺序正确

2. 性能考量：
   - 对于复杂UI，建议将帧率控制在合理范围（15-30fps）

## 常见问题

### Q: 图集模式和精灵模式有什么区别？

A: 图集模式从UIAtlas组件中按前缀筛选精灵，适合管理大量动画；精灵模式直接使用指定的精灵数组，更直观但需要手动设置每一帧。图集模式便于资源管理，精灵模式则更灵活。

### Q: 如何让动画只播放一次？

A: 将Loop属性设置为false即可让动画播放一次后停止：

```csharp
// 获取组件引用
UISpriteAnimation animation = GetComponent<UISpriteAnimation>();

// 设置不循环
animation.Loop = false;
```

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
