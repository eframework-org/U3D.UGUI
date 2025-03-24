# UIAtlas

`UIAtlas` 是一个用于管理 Sprite 资源的集中式工具，它提供了便捷的精灵图集管理功能，可以有效提高 UI 资源的组织和访问效率。

## 功能特性

- 集中管理多个 Sprite 资源，便于统一访问和维护
- 提供高效的 Sprite 名称查找功能，支持通过名称快速检索资源
- 自动缓存 Sprite 索引，优化重复查询性能
- 与 UIAtlasEditor 配合使用，支持自动导入和更新
- 在运行时提供稳定的资源访问接口

## 使用手册

### 创建图集

1. 通过编辑器创建：
   - 在 Project 窗口中右键点击目标文件夹 -> 选择 "Create/2D/Sheet Atlas"
   - 在弹出的对话框中选择包含精灵图片的文档目录
   - 系统将自动创建 UIAtlas 预制体

2. 图集组成：
   - `RawPath`: 指向包含原始精灵图片的目录
   - `Sprites`: 存储所有可用的 Sprite 资源数组，通常由 UIAtlasEditor 自动填充

### 获取 Sprite

```csharp
// 获取图集组件
UIAtlas atlas = GetComponent<UIAtlas>();
// 或者加载图集资源
UIAtlas atlas = Resources.Load<UIAtlas>("UI/CommonAtlas");

// 通过名称获取 Sprite
Sprite iconSprite = atlas.GetSprite("IconName");

// 使用获取的 Sprite
image.sprite = iconSprite;
```

### 编辑器工具

UIAtlasEditor 提供了以下功能：

1. 自定义图标：
   - 在项目窗口中为 UIAtlas 预制体显示自定义图标，便于识别

2. 自动导入：
   - 当 UIAtlas 预制体被导入或移动时，系统将自动处理其依赖关系
   - 使用 TexturePacker 处理图集，支持精灵图集的自动分割和重组

3. 图集管理：
   - 管理纹理导入设置和精灵元数据
   - 支持透明度处理和边界修剪
   - 根据文档目录自动更新图集内容

## 常见问题

### Q: 如何更新已有图集中的精灵？

A: 只需在文档目录中更新或添加精灵图片，然后在 Unity 编辑器中重新导入 UIAtlas 预制体即可。UIAtlasEditor 会自动处理图集的更新。

### Q: 图集中的 Sprite 可以动态添加吗？

A: 在编辑器模式下可以通过更新文档目录并重新导入来添加精灵。在运行时，图集内容是只读的，建议在设计阶段合理规划图集内容。

### Q: 使用 UIAtlas 与直接引用 Sprite 相比有什么优势？

A: 使用 UIAtlas 可以：
- 集中管理 UI 资源，便于维护和更新
- 通过名称访问资源，避免直接引用依赖
- 利用缓存机制提高频繁访问的性能
- 简化资源更新流程，只需更新文档目录内容

### Q: 如何处理找不到指定名称的 Sprite 的情况？

A: `GetSprite` 方法在找不到指定名称的 Sprite 时会返回 `null`。建议在使用前进行空检查：

```csharp
Sprite sprite = atlas.GetSprite("IconName");
if (sprite != null)
{
    image.sprite = sprite;
}
else
{
    Debug.LogWarning($"Sprite 'IconName' not found in atlas.");
}
```

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 技术细节

- 首次调用 `GetSprite` 方法时会创建名称到索引的缓存字典，提高后续访问性能
- UIAtlasEditor 使用 TexturePacker 处理图集，支持边界修剪、透明度处理等高级功能
- 图集导入过程会自动设置合适的纹理导入设置，如禁用 Mipmap、设置 Sprite 模式等

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
