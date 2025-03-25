# UIAtlas

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)

UIAtlas 是一个用于管理 Sprite 资源的组件，提供了 TexturePacker 图集打包与 Sprite 查找功能。

## 功能特性

- 图集资源导入：基于 TexturePacker 实现了自动图集导入功能
- 图集资源管理：通过名称检索 Sprite 资源，优化重复查询性能

## 使用手册

### 1. 创建图集

通过编辑器创建图集：

1. 在 `Project` 窗口中选择目标文件夹
2. 右键 `Create/2D/Sheet Atlas`
3. 选择包含精灵图片的素材目录
4. 编辑器将自动创建 `UIAtlas` 预制体

### 2. 资源导入

支持自动和手动两种方式导入图集：

1. 监听资源导入事件触发自动化导入流程
2. 右键 `UIAtlas` 资源并 `Reimport`

### 3. 图集使用

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

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
