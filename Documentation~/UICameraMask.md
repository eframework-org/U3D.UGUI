# UICameraMask

`UICameraMask` 是一个提供屏幕淡入淡出效果的组件，主要用于场景过渡。通过单例模式设计，可在任何地方方便地控制全屏遮罩效果。

## 功能特性

- 提供全屏幕遮罩淡入淡出效果
- 支持自定义遮罩颜色和透明度
- 单例模式设计，便于全局调用
- 可控制过渡时间，适用于各种场景切换

## 使用手册

### 基本设置

1. 创建遮罩：
   - 在Canvas下创建一个Image组件作为全屏遮罩
   - 添加CanvasGroup组件
   - 添加UICameraMask组件，并将CanvasGroup赋值给ScreenMask字段

2. 全局访问：
   - 通过静态属性Instance可在任何地方访问相机遮罩
   - 例如：`UICameraMask.Instance.SetTarget(Color.black, 2f);`

### 代码控制

```csharp
// 场景过渡示例
// 在加载新场景前使用黑色遮罩，2秒淡入淡出
UICameraMask.Instance.SetTarget(Color.black, 2f);

// 使用红色遮罩，0.5秒淡入淡出
UICameraMask.Instance.SetTarget(Color.red, 0.5f);

// 使用半透明黑色遮罩，1秒淡入淡出，最大不透明度为0.8
UICameraMask.Instance.SetTarget(Color.black, 1f, 0.8f);
```

### 常见用例

1. 场景过渡：
   - 在加载新场景前调用：`UICameraMask.Instance.SetTarget(Color.black);`
   - 场景加载完成后遮罩会自动淡出

2. 自定义过渡效果：
   - 可以设置不同的颜色来创造不同的氛围
   - 可以调整过渡时间控制淡入淡出速度
   - 可以设置最大透明度控制遮罩的不透明程度

3. 特殊效果：
   - 用于模拟闪光效果
   - 用于实现昼夜交替的过渡
   - 用于表现角色失去意识的黑屏效果

## 常见问题

### Q: 如何使用UICameraMask实现闪烁效果？

A: 可以通过连续调用SetTarget方法并使用短暂的过渡时间来实现闪烁效果：

```csharp
IEnumerator FlashEffect(int flashCount, float flashDuration)
{
    for (int i = 0; i < flashCount; i++)
    {
        UICameraMask.Instance.SetTarget(Color.white, flashDuration, 0.7f);
        yield return new WaitForSeconds(flashDuration * 2);
    }
}
```

### Q: 为什么我设置了遮罩但没有效果？

A: 请确保：
1. ScreenMask字段已正确赋值给一个CanvasGroup组件
2. CanvasGroup所在的Canvas处于正确的渲染顺序（通常需要位于Overlay模式或高Sort Order）
3. Image组件覆盖了整个屏幕区域

### Q: 遮罩可以只覆盖屏幕的一部分吗？

A: UICameraMask设计为全屏幕遮罩组件。如果需要部分遮罩效果，可以调整Image组件的RectTransform大小或者使用其他UI遮罩组件。

### Q: 如何暂停或取消正在进行的淡入淡出效果？

A: 当前版本没有直接取消正在进行的淡入淡出效果的方法。一个变通方法是立即调用另一个SetTarget，设置极短的过渡时间和所需的最终透明度。

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 组件内部使用Update函数处理渐变效果
- 通过控制CanvasGroup的alpha属性实现透明度变化
- 使用Image组件控制遮罩的颜色
- 完整的淡入淡出过程：先淡入（从透明到不透明），然后自动淡出（从不透明到透明）

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
