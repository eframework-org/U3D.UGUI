// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using UnityEngine;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UI 图集组件，用于管理和获取 Sprite 资源。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 集中管理多个 Sprite 资源，便于统一访问
    /// - 提供高效的 Sprite 名称查找功能
    /// - 自动缓存 Sprite 索引，优化重复查询性能
    /// - 与 UIAtlasEditor 配合使用，支持自动导入和更新
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
