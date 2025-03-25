# UIButtonScale

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIButtonScale 是一个用于给按钮添加缩放交互反馈效果的组件，当按钮被按下时自动缩放按钮，松开时恢复原始大小，提供了良好的视觉反馈。

## 功能特性

- 为按钮提供简单而有效的视觉反馈机制
- 自动处理指针按下和抬起事件
- 支持自定义缩放比例和过渡效果

## 使用手册

### 基本设置

1. 添加组件：
   - 选择需要添加缩放效果的按钮对象
   - 在 Inspector 窗口点击 "Add Component"
   - 选择 "UI/Button Scale" 或搜索 "UIButtonScale"

2. 组件配置：
   - `Button`：按钮组件引用，默认自动获取当前对象上的 Button 组件
   - `Scale`：按下时的缩放比例，默认为 0.95

### 运行时行为

组件会自动监听按钮的指针事件：
- 当按下时，按钮会缩小到设定的比例
- 当指针抬起时，按钮会恢复到原始大小
- 缩放效果是即时的，没有过渡动画

## 常见问题

### Q: 是否支持动画过渡效果？

A: 基础版本是即时缩放，没有过渡动画。如果需要平滑过渡效果，可以考虑使用 DOTween 等插件，或自定义扩展组件。

### Q: 该组件与按钮的其他交互效果是否兼容？

A: 兼容。UIButtonScale 只影响按钮的缩放，不会干扰颜色、图片切换等其他交互效果。

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
