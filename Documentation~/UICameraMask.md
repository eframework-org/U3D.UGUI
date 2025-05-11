# UICameraMask

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![DeepWiki](https://img.shields.io/badge/DeepWiki-Explore-blue)](https://deepwiki.com/eframework-org/U3D.UGUI)

UICameraMask 是一个屏幕淡入淡出效果的控制组件，主要用于场景过渡，基于单例模式可以全局控制全屏遮罩效果。

## 功能特性

- 提供全屏幕遮罩淡入淡出效果
- 支持自定义遮罩颜色和透明度
- 单例模式设计，便于全局调用

## 使用手册

### 1. 基本使用

1. 添加组件：
  - 在 Canvas 下创建一个 Image 组件作为全屏遮罩
  - 添加 CanvasGroup 组件
  - 添加 UICameraMask 组件，并将 CanvasGroup 赋值给 ScreenMask 字段

2. 代码控制
```csharp
// 使用半透明黑色遮罩，1秒淡入淡出，最大不透明度为0.8
UICameraMask.Instance.SetTarget(Color.black, 1f, 0.8f);
```

### 2. 常见用例

1. 自定义过渡效果：
  - 可以设置不同的颜色来创造不同的氛围
  - 可以调整过渡时间控制淡入淡出速度
  - 可以设置最大透明度控制遮罩的不透明程度

2. 特殊效果：
  - 用于模拟闪光效果
  - 用于表现角色失去意识的黑屏效果

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
