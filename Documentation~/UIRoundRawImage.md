# UIRoundRawImage

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIRoundRawImage 是一个圆角图片组件，继承自 RawImage 组件，通过自定义网格实现图片的圆角效果。

## 功能特性

- 自定义圆角半径和平滑度
- 自动计算和生成圆角网格

## 使用手册

### 1. 基本使用

1. 添加组件：
  - 在 `Hierarchey` 面板中选择或创建一个游戏对象
  - 在 `Inspector` 窗口点击 `Add Component`
  - 选择 `UI/Round Raw Image` 以添加组件
  - 组件会自动替代默认的 `RawImage`，提供圆角效果

2. 代码控制：
   ```csharp
   // 控制圆角大小（值越小圆角越大，默认为2）
   roundRawImage.Radius = 2f;
   
   // 控制圆角平滑度（值越大越平滑，默认为10）
   roundRawImage.TriangleNum = 10;
   ```

### 2. 注意事项

1. 性能考量：
   - TriangleNum 数值不宜设置过高，会增加渲染开销
   - 对于不需要高度平滑的圆角，推荐保持默认值

2. 尺寸调整：
   - 在调整 RectTransform 的大小时，圆角会自动适应
   - 在极端宽高比下可能需要手动调整 Radius 以获得更好的效果

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
