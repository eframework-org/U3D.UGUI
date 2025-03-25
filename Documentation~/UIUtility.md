# UIUtility

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIUtility 是一个 Unity UI 的工具函数集，提供了一系列简化 UI 组件操作的扩展方法，包含组件查找、属性设置、事件处理等功能。

## 功能特性

- 快速索引功能：通过路径快速获取 UI 组件的子对象
- 显示状态控制：支持透明度、文本、显示隐藏等属性的设置
- 扩展方法支持：采用扩展方法设计，函数调用更为直观

## 使用手册

### 1. 组件查找

1. 快速索引：
   ```csharp
   // 通过RectTransform查找子组件
   Button btn = rectTransform.Index<Button>("ButtonName");
   
   // 通过Canvas查找子组件
   Text txt = canvas.Index<Text>("TextName");
   ```

### 2. 透明度控制

1. 设置组件透明度：
   ```csharp
   // 设置当前组件透明度为0.5
   gameObject.SetGraphicAlpha(0.5f);
   
   // 设置子组件透明度为128（0-255范围）
   gameObject.SetGraphicAlpha("ChildName", 128);
   ```

### 3. 事件管理

1. 按钮点击事件：
   ```csharp
   // 为按钮设置点击事件
   gameObject.SetButtonEvent((go) => {
       Debug.Log("按钮被点击：" + go.name);
   });
   
   // 启用/禁用按钮交互
   gameObject.SetEventEnabled(false);
   ```

### 4. 文本设置

1. 设置普通文本：
   ```csharp
   // 设置Text组件内容
   gameObject.SetLabelText("新文本内容");
   
   // 设置子对象的Text组件内容
   gameObject.SetLabelText("ChildText", "新文本内容");
   ```

2. 设置TMP文本：
   ```csharp
   // 设置TextMeshProUGUI组件内容
   gameObject.SetMeshProText("新文本内容");
   
   // 设置子对象的TextMeshProUGUI组件内容
   gameObject.SetMeshProText("ChildTMP", "新文本内容");
   ```

### 5. 图片管理

1. 设置精灵图片：
   ```csharp
   // 从图集中设置图片
   gameObject.SetSpriteName("ChildImage", "SpriteName", atlasComponent);
   
   // 设置图片透明度
   gameObject.SetSpriteAlpha(0.5f);
   ```

2. 加载网络图片：
   ```csharp
   // 加载并显示网络图片
   gameObject.SetRawImage("http://example.com/image.jpg");
   
   // 加载网络图片并禁用缓存
   gameObject.SetRawImage("http://example.com/image.jpg", false);
   
   // 获取已缓存的贴图
   Texture2D tex = UIUtility.GetTexture("http://example.com/image.jpg");
   ```

### 6. 布局刷新

1. 刷新UI布局：
   ```csharp
   // 重新计算布局
   gameObject.RefreshObSort();
   
   // 刷新子对象布局
   gameObject.RefreshObSort("ChildLayout");
   ```

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
