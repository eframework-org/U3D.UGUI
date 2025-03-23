# UISpriteAnimation

`UISpriteAnimation` 是一个UI精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。

## 功能特性

- 支持两种加载模式：图集模式和精灵模式
- 支持按前缀筛选图集中的精灵
- 可自定义动画帧率和循环播放设置
- 支持在编辑器模式下预览动画效果
- 自动适配原始尺寸，保证精灵动画比例正确

## 使用手册

### 组件添加

1. 添加组件：
   - 选择UI游戏对象（必须有Image组件）
   - 在Inspector窗口点击"Add Component"
   - 选择"UI/Sprite Animation"或搜索"UISpriteAnimation"
   - 组件会自动获取Image组件用于显示动画

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
   - 使用Native Size选项时要注意可能改变UI元素大小

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

// 开始播放
animation.Reset();
```

### Q: 如何在动画播放完毕后执行操作？

A: 当前版本没有提供动画完成事件，但可以通过监控Active状态来判断非循环动画是否播放完毕：

```csharp
// 在Update方法中检查
void Update() {
    if (previouslyActive && !animation.Active) {
        // 动画刚刚结束
        OnAnimationComplete();
    }
    previouslyActive = animation.Active;
}
```

### Q: 如何调整动画播放速度？

A: 通过修改FrameRate属性可以控制动画播放速度，值越大播放越快：

```csharp
// 设置较慢的播放速度
animation.FrameRate = 10; // 每秒10帧

// 设置较快的播放速度
animation.FrameRate = 60; // 每秒60帧
```

### Q: 使用图集模式时，如何确保精灵按正确顺序播放？

A: 由于图集模式下精灵按名称字母顺序排序，建议使用数字前缀命名精灵，确保排序正确：
- 错误示例：run_1, run_10, run_2（会排序为run_1, run_10, run_2）
- 正确示例：run_01, run_02, run_10（会排序为run_01, run_02, run_10）

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 实现了执行编辑模式下的预览功能（通过ExecuteInEditMode特性）
- 通过RequireComponent特性确保组件附加在有Image组件的对象上
- 使用两种动画帧加载策略：
  - 图集模式：从UIAtlas筛选符合前缀的精灵，按名称排序
  - 精灵模式：直接使用指定的Sprite数组
- 使用时间累积器(delta)计算帧切换时机，确保帧率准确
- 支持动态调整Image尺寸以匹配精灵原始大小（通过nativeSize选项）

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
