# UIUtility

`UIUtility` 是一个UI工具类，提供了一系列便捷的UI操作方法，包含组件查找、属性设置、事件处理和资源加载等功能。

## 功能特性

- 提供UI组件快速查找和索引功能
- 支持透明度、颜色等属性的便捷设置
- 集成按钮点击事件和状态管理
- 简化文本内容和图片设置操作
- 支持网络图片异步加载和缓存管理
- 提供UI布局刷新功能

## 使用手册

### 组件查找

1. 快速索引：
   ```csharp
   // 通过RectTransform查找子组件
   Button btn = rectTransform.Index<Button>("ButtonName");
   
   // 通过Canvas查找子组件
   Text txt = canvas.Index<Text>("TextName");
   ```

### 透明度控制

1. 设置组件透明度：
   ```csharp
   // 设置当前组件透明度为0.5
   gameObject.SetGraphicAlpha(0.5f);
   
   // 设置子组件透明度为128（0-255范围）
   gameObject.SetGraphicAlpha("ChildName", 128);
   ```

### 事件管理

1. 按钮点击事件：
   ```csharp
   // 为按钮设置点击事件
   gameObject.SetButtonEvent((go) => {
       Debug.Log("按钮被点击：" + go.name);
   });
   
   // 启用/禁用按钮交互
   gameObject.SetEventEnabled(false);
   ```

### 文本设置

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

### 图片管理

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

### 布局刷新

1. 刷新UI布局：
   ```csharp
   // 重新计算布局
   gameObject.RefreshObSort();
   
   // 刷新子对象布局
   gameObject.RefreshObSort("ChildLayout");
   ```

## 常见问题

### Q: UIUtility中的Index方法和GameObject.Find有什么区别？

A: UIUtility的Index方法专为UI组件设计，是GameObject.Find和GetComponent的组合优化版本，可以直接获取指定路径下的特定类型组件。它更高效，因为不需要分开调用Find和GetComponent，而且支持泛型形式，使用更简洁。

### Q: 如何正确处理网络图片加载失败的情况？

A: 使用SetRawImage加载网络图片时，可以添加额外的检查和回调机制：

```csharp
// 首先尝试获取已缓存的贴图
Texture2D cached = UIUtility.GetTexture(imageUrl);
if (cached != null) {
    // 缓存中存在，直接使用
    rawImage.texture = cached;
} else {
    // 显示加载中图片
    rawImage.texture = loadingTexture;
    
    // 加载网络图片
    gameObject.SetRawImage(imageUrl);
    
    // 在适当的地方检查加载是否成功
    StartCoroutine(CheckImageLoaded(imageUrl, rawImage, fallbackTexture));
}

// 检查图片是否加载成功的协程
IEnumerator CheckImageLoaded(string url, RawImage image, Texture fallback) {
    yield return new WaitForSeconds(5f); // 等待合理的加载时间
    if (image.texture == null || image.texture == loadingTexture) {
        // 加载失败，使用备用图片
        image.texture = fallback;
    }
}
```

### Q: 如何批量设置多个UI元素的文本内容？

A: 可以结合Index方法和SetLabelText方法批量设置：

```csharp
// 假设有一个包含多个Text组件的父对象
GameObject textContainer = transform.Find("TextContainer").gameObject;

// 设置文本内容
string[] contents = { "标题", "描述", "详情" };
string[] paths = { "Title", "Description", "Detail" };

for (int i = 0; i < paths.Length; i++) {
    textContainer.SetLabelText(paths[i], contents[i]);
}
```

### Q: 透明度设置方法有多种参数形式，应该使用哪一种？

A: UIUtility提供了多种透明度设置方法，选择取决于你的具体需求：

- 如果你习惯使用0-1范围的float值：`SetGraphicAlpha(0.5f)`
- 如果你习惯使用0-255范围的int值：`SetGraphicAlpha(128)`
- 如果需要设置子对象：`SetGraphicAlpha("ChildPath", 0.5f)` 或 `SetGraphicAlpha("ChildPath", 128)`

选择最符合你当前工作流程的方法即可，它们最终都会转换为0-1范围的alpha值。

### Q: 使用SetRawImage方法加载大量图片会有性能问题吗？

A: 是的，不恰当地加载大量网络图片可能导致性能问题。建议：

1. 利用缓存机制：默认情况下SetRawImage会缓存贴图
2. 控制同时加载的图片数量：实现队列机制
3. 根据需要调整图片尺寸和质量
4. 对不在视图中的图片组件停止加载或延迟加载
5. 考虑使用对象池管理RawImage对象

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 所有方法都实现为扩展方法，可以直接在GameObject、Component或其他Unity对象上调用
- Index方法内部使用XComp工具类，提供高效的组件查找
- 图片加载使用UnityWebRequestTexture异步加载网络资源
- 使用静态字典缓存已加载的网络贴图，避免重复下载
- SetButtonEvent方法利用了Unity UI的事件系统，简化了按钮点击逻辑
- RefreshObSort方法支持多种布局控制器，包括ContentSizeFitter和HorizontalLayoutGroup
- 文本设置方法同时支持Unity原生Text组件和TextMeshPro的TextMeshProUGUI组件

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
