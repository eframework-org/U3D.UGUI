# UICameraMask

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UICameraMask 是一个提供屏幕淡入淡出效果的组件，主要用于场景过渡。通过单例模式设计，可在任何地方方便地控制全屏遮罩效果。

## 功能特性

- 提供全屏幕遮罩淡入淡出效果
- 支持自定义遮罩颜色和透明度
- 单例模式设计，便于全局调用

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
// 使用半透明黑色遮罩，1秒淡入淡出，最大不透明度为0.8
UICameraMask.Instance.SetTarget(Color.black, 1f, 0.8f);
```

### 常见用例

1. 自定义过渡效果：
   - 可以设置不同的颜色来创造不同的氛围
   - 可以调整过渡时间控制淡入淡出速度
   - 可以设置最大透明度控制遮罩的不透明程度

2. 特殊效果：
   - 用于模拟闪光效果
   - 用于表现角色失去意识的黑屏效果

## 常见问题

### Q: 为什么我设置了遮罩但没有效果？

A: 请确保：
1. ScreenMask字段已正确赋值给一个CanvasGroup组件
2. CanvasGroup所在的Canvas处于正确的渲染顺序（通常需要位于Overlay模式或高Sort Order）
3. Image组件覆盖了整个屏幕区域

### Q: 如何暂停或取消正在进行的淡入淡出效果？

A: 当前版本没有直接取消正在进行的淡入淡出效果的方法。一个变通方法是立即调用另一个SetTarget，设置极短的过渡时间和所需的最终透明度。

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
