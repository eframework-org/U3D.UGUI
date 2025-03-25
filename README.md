# U3D.UGUI

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

U3D.UGUI 是基于 Unity UI 的一个拓展模块，提供了丰富的组件扩展和工具集，例如 Sheet 图集打包和循环列表视图等，能够有效简化 UI 开发流程。

## 功能特性

- [UIAtlas](Documentation~/UIAtlas.md) UIAtlas 是一个用于管理 Sprite 资源的集中式工具，提供了便捷的精灵图集管理功能，可以有效提高 UI 资源的组织和访问效率。
- [UIButtonScale](Documentation~/UIButtonScale.md) UIButtonScale 是一个用于给按钮添加缩放交互反馈效果的组件，当按钮被按下时自动缩放按钮，松开时恢复原始大小，提供了良好的视觉反馈。
- [UICameraMask](Documentation~/UICameraMask.md) UICameraMask 是一个提供屏幕淡入淡出效果的组件，主要用于场景过渡。通过单例模式设计，可在任何地方方便地控制全屏遮罩效果。
- [UIEventListener](Documentation~/UIEventListener.md) UIEventListener 是一个UI事件监听器组件，用于处理UI交互的各种事件。实现了Unity UI事件系统的多种接口。
- [UIRoundRawImage](Documentation~/UIRoundRawImage.md) UIRoundRawImage 是一个圆角原始图片组件，继承自Unity的RawImage，通过自定义网格实现图片的圆角效果。
- [UISpriteAnimation](Documentation~/UISpriteAnimation.md) UISpriteAnimation 是一个UI精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。
- [UIUtility](Documentation~/UIUtility.md) UIUtility 是一个UI工具类，提供了一系列便捷的UI操作方法，包含组件查找、属性设置、事件处理和资源加载等功能。
- [UIWrapContent](Documentation~/UIWrapContent.md) UIWrapContent 是一个循环列表内容组件，扩展自ScrollRect，提供高效的大数据列表和网格显示能力，通过元素复用降低内存占用。

## 常见问题

更多问题，请查阅[问题反馈](CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](CHANGELOG.md)
- [贡献指南](CONTRIBUTING.md)
- [许可证](LICENSE.md) 