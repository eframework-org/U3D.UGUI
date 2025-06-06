# EFramework UnityUI Plugins

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![DeepWiki](https://img.shields.io/badge/DeepWiki-Explore-blue)](https://deepwiki.com/eframework-org/U3D.UGUI)

EFramework UnityUI Plugins 是基于 Unity UI 的一个拓展插件，提供了丰富的组件扩展和工具集，例如 TexturePacker 图集打包和循环列表视图等，能够有效简化 UI 开发流程。

## 功能特性

- [UIAtlas](Documentation~/UIAtlas.md) 是一个用于管理 Sprite 资源的组件，提供了 TexturePacker 图集打包与 Sprite 查找功能
- [UIButtonScale](Documentation~/UIButtonScale.md) 实现了按钮缩放的交互效果，支持按钮按下时自动缩放，松开时恢复原始大小，提供了良好的视觉反馈
- [UICameraMask](Documentation~/UICameraMask.md) 是一个屏幕淡入淡出效果的控制组件，主要用于场景过渡，基于单例模式可以全局控制全屏遮罩效果
- [UIEventListener](Documentation~/UIEventListener.md) 是一个 UI 事件监听器组件，封装了多种 Unity UI 事件接口，用于处理 UI 交互的各种事件
- [UIRoundRawImage](Documentation~/UIRoundRawImage.md) 是一个圆角图片组件，继承自 RawImage 组件，通过自定义网格实现图片的圆角效果
- [UISpriteAnimation](Documentation~/UISpriteAnimation.md) 是一个 UI 精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧
- [UIUtility](Documentation~/UIUtility.md) 是一个 Unity UI 的工具函数集，提供了一系列简化 UI 组件操作的扩展方法，包含组件查找、属性设置、事件处理等功能
- [UIWrapContent](Documentation~/UIWrapContent.md) 是一个拓展自 ScrollRect 的循环列表视图组件，提供了高效的大数据列表和网格显示能力，通过元素复用降低内存占用

## 常见问题

更多问题，请查阅[问题反馈](CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](CHANGELOG.md)
- [贡献指南](CONTRIBUTING.md)
- [许可证](LICENSE.md)