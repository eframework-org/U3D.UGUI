# UIRoundRawImage

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIRoundRawImage 是一个圆角原始图片组件，继承自Unity的RawImage，通过自定义网格实现图片的圆角效果。

## 功能特性

- 为UI原始图片添加圆角效果
- 自动计算和生成圆角网格
- 支持自定义圆角半径和平滑度

## 使用手册

### 组件添加

1. 添加到游戏对象：
   - 在UI界面中选择或创建一个游戏对象
   - 在Inspector窗口点击"Add Component"
   - 选择"UI/Round Raw Image"或搜索"UIRoundRawImage"
   - 组件会自动替代默认的RawImage，提供圆角效果

### 属性设置

1. 自定义圆角效果：
   ```csharp
   // 控制圆角大小（值越小圆角越大，默认为2）
   roundRawImage.Radius = 2f;
   
   // 控制圆角平滑度（值越大越平滑，默认为10）
   roundRawImage.TriangleNum = 10;
   ```

### 注意事项

1. 性能考量：
   - TriangleNum数值不宜设置过高，会增加渲染开销
   - 对于不需要高度平滑的圆角，推荐保持默认值

2. 尺寸调整：
   - 在调整RectTransform的大小时，圆角会自动适应
   - 在极端宽高比下可能需要手动调整Radius以获得更好的效果

## 常见问题

### Q: UIRoundRawImage和普通RawImage有什么区别？

A: 普通RawImage显示矩形图像，而UIRoundRawImage在保留RawImage所有功能的同时，通过自定义网格生成圆角效果，使UI界面更加美观。

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
