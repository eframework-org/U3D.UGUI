# UIButtonScale

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![DeepWiki](https://img.shields.io/badge/DeepWiki-Explore-blue)](https://deepwiki.com/eframework-org/U3D.UGUI)

UIButtonScale 实现了按钮缩放的交互效果，支持按钮按下时自动缩放，松开时恢复原始大小，提供了良好的视觉反馈。

## 功能特性

- 自定义缩放比例和过渡效果
- 自动处理按下和抬起事件

## 使用手册

### 1. 基本使用

1. 添加组件：
  - 选择需要添加缩放效果的按钮对象
  - 在 Inspector 窗口点击 "Add Component"
  - 选择 "UI/Button Scale" 或搜索 "UIButtonScale"

2. 组件配置：
  - `Button`：按钮组件引用，默认自动获取当前对象上的 Button 组件
  - `Scale`：按下时的缩放比例，默认为 0.95

### 2. 实现原理

组件会自动监听按钮的指针事件：
  - 当按下时，按钮会缩小到设定的比例
  - 当指针抬起时，按钮会恢复到原始大小
  - 缩放效果是即时的，没有过渡动画

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
