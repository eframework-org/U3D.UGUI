# UIRoundRawImage

`UIRoundRawImage` 是一个圆角原始图片组件，继承自Unity的RawImage，通过自定义网格实现图片的圆角效果，无需额外的Shader即可应用。

## 功能特性

- 为UI原始图片添加圆角效果
- 自动计算和生成圆角网格
- 支持自定义圆角半径和平滑度
- 无需额外的Shader即可实现圆角效果

## 使用手册

### 组件添加

1. 添加到游戏对象：
   - 在UI界面中选择或创建一个游戏对象
   - 在Inspector窗口点击"Add Component"
   - 选择"UI/Round Raw Image"或搜索"UIRoundRawImage"
   - 组件会自动替代默认的RawImage，提供圆角效果

### 属性设置

1. 基本属性：
   ```csharp
   // 设置显示的纹理
   roundRawImage.texture = myTexture;
   
   // 设置图片的颜色
   roundRawImage.color = Color.white;
   
   // 设置自定义材质（可选）
   roundRawImage.material = myMaterial;
   ```

2. 自定义圆角效果：
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

### Q: 如何调整圆角的大小？

A: UIRoundRawImage通过Radius参数控制圆角大小，默认值为2。值越小圆角效果越明显，值越大圆角效果越不明显。理论上，Radius为0时会变成完全的圆形。

```csharp
// 获取组件引用
UIRoundRawImage roundImage = GetComponent<UIRoundRawImage>();

// 设置小圆角（值越小圆角越大）
roundImage.Radius = 1.5f;

// 设置大圆角（值越大圆角越小）
roundImage.Radius = 3f;
```

### Q: 圆角的平滑度如何调整？

A: 通过TriangleNum参数可以控制圆角的平滑度，默认值为10。值越大圆角边缘越平滑，但同时也会增加渲染开销。一般情况下，10-20之间的值能提供良好的平滑效果。

```csharp
// 增加平滑度
roundImage.TriangleNum = 20;

// 降低平滑度（提高性能）
roundImage.TriangleNum = 5;
```

### Q: 在修改RectTransform大小后，圆角变形了怎么办？

A: UIRoundRawImage会根据当前的宽高比自动计算圆角，但在极端宽高比下可能需要手动调整Radius参数。一般来说，保持Radius值在1.5-3之间，可以在大多数情况下获得良好的圆角效果。

### Q: 可以只有部分角为圆角吗？

A: 当前版本的UIRoundRawImage会对所有四个角应用相同的圆角效果。如果需要特定角的圆角，可能需要扩展此组件或使用其他替代方案（如使用带有特殊蒙版的Image）。

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 继承自Unity UI的RawImage组件，保留所有RawImage的功能
- 通过重写OnPopulateMesh方法自定义网格生成过程
- 使用三角形组合创建平滑的圆角效果：
  - 中间区域使用普通矩形
  - 四角使用扇形区域，通过细分三角形实现曲线效果
- 自动计算UV坐标，确保纹理正确映射到圆角图形上
- 圆角半径会根据组件尺寸动态调整，确保在不同大小下呈现一致的视觉效果

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
