# UISpriteAnimation

[![Version](https://img.shields.io/npm/v/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![Downloads](https://img.shields.io/npm/dm/org.eframework.u3d.ugui)](https://www.npmjs.com/package/org.eframework.u3d.ugui)
[![DeepWiki](https://img.shields.io/badge/DeepWiki-Explore-blue)](https://deepwiki.com/eframework-org/U3D.UGUI)

UISpriteAnimation 是一个 UI 精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。

## 功能特性

- 加载模式：支持图集和精灵图片两种加载模式
- 参数控制：支持自定义动画帧率和循环播放等设置

## 使用手册

### 1. 基本使用

添加组件：
  - 在 `Hierarchey` 面板中选择或创建一个游戏对象（必须有Image组件）
  - 在 `Inspector` 窗口点击 `Add Component`
  - 选择 `UI/Sprite Animation` 以添加组件

### 2. 模式选择

1. 图集模式(Atlas)：
   - 从 UIAtlas 组件加载精灵
   - 确保游戏对象上有 UIAtlas 组件
   - 设置 Prefix(前缀) 筛选图集中的精灵
   - 匹配前缀的精灵将按名称排序组成动画序列

2. 精灵模式(Sprite)：
   - 直接使用精灵数组
   - 将精灵数组赋值给 Sprites 属性
   - 精灵将按数组顺序播放

### 3. 动画设置

1. 基本设置：
   ```csharp
   // 设置动画帧率（每秒帧数）
   spriteAnimation.FrameRate = 30;
   
   // 设置是否循环播放
   spriteAnimation.Loop = true;
   
   // 设置精灵数组（精灵模式下）
   spriteAnimation.Sprites = mySprites;
   
   // 设置精灵名称前缀（图集模式下）
   spriteAnimation.Prefix = "run_";
   ```

2. 运行控制：
   ```csharp
   // 重置动画到第一帧并重新开始播放
   spriteAnimation.Reset();
   
   // 检查动画是否处于活动状态
   if (spriteAnimation.Active) {
       Debug.Log("动画正在播放");
   }
   
   // 获取动画总帧数
   int totalFrames = spriteAnimation.Frames;
   ```

## 常见问题

更多问题，请查阅[问题反馈](../CONTRIBUTING.md#问题反馈)。

## 项目信息

- [更新记录](../CHANGELOG.md)
- [贡献指南](../CONTRIBUTING.md)
- [许可证](../LICENSE)
