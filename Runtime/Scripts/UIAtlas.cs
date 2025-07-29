// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using UnityEngine;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UIAtlas 是一个用于管理 Sprite 资源的组件，提供了 TexturePacker 图集打包与 Sprite 查找功能。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 图集资源导入：基于 TexturePacker 实现了自动图集导入功能
    /// - 图集资源管理：通过名称检索 Sprite 资源，优化重复查询性能
    /// 
    /// 使用手册
    /// 1. 前置条件
    ///     
    ///     使用图集打包功能需要安装以下依赖：
    ///     1. 下载安装 [TexturePacker](https://www.codeandweb.com/texturepacker) 软件并**同意**许可协议
    ///     2. Docker 环境使用可参考 TexturePacker 的[ CI 教程](https://www.codeandweb.com/texturepacker/documentation/docker-ci)
    /// 
    /// 2. 创建图集
    /// 
    ///     通过编辑器创建图集：
    ///     1. 在 Project 窗口中选择目标文件夹
    ///     2. 右键 Create/2D/Sheet Atlas
    ///     3. 选择包含精灵图片的素材目录
    ///     4. 编辑器将自动创建 UIAtlas 预制体
    /// 
    /// 3. 资源导入
    /// 
    ///     支持自动和手动两种方式导入图集：
    ///     1. 监听资源导入事件触发自动化导入流程
    ///     2. 右键 UIAtlas 资源并 Reimport
    /// 
    /// 4. 图集使用
    /// 
    ///     // 获取图集组件
    ///     var atlas = GetComponent&lt;UIAtlas&gt;();
    /// 
    ///     // 或者加载图集资源
    ///     var atlas = Resources.Load&lt;UIAtlas&gt;("UI/CommonAtlas");
    /// 
    ///     // 通过名称获取 Sprite
    ///     var iconSprite = atlas.GetSprite("IconName");
    /// 
    ///     // 使用获取的 Sprite
    ///     image.sprite = iconSprite;
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [AddComponentMenu("UI/Atlas")]
    public class UIAtlas : MonoBehaviour
    {
        /// <summary>
        /// 素材路径，指向包含原始精灵图片的目录。
        /// </summary>
        public string RawPath;

        /// <summary>
        /// Sprite 资源数组，存储所有可用的 Sprite。
        /// 通常由 UIAtlasEditor 自动填充。
        /// </summary>
        public Sprite[] Sprites;

        /// <summary>
        /// Sprite 名称到索引的缓存字典，用于加速 Sprite 查找。
        /// </summary>
        private Dictionary<string, int> spriteCache;

        /// <summary>
        /// 根据名称获取对应的 Sprite 资源。
        /// 首次调用时会创建缓存，提高后续访问性能。
        /// </summary>
        /// <param name="name">要获取的 Sprite 名称</param>
        /// <returns>如果找到则返回对应的 Sprite，否则返回 null</returns>
        public Sprite GetSprite(string name)
        {
            if (spriteCache == null)
            {
                spriteCache = new();
                if (Sprites != null && Sprites.Length > 0)
                {
                    for (int i = 0; i < Sprites.Length; i++)
                    {
                        spriteCache.Add(Sprites[i].name, i);
                    }
                }
            }
            if (spriteCache.TryGetValue(name, out var idx))
            {
                return Sprites[idx];
            }
            return null;
        }
    }
}
