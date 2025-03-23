// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EFramework.Editor;

namespace EFramework.UnityUI.Editor
{
    /// <summary>
    /// Unity Canvas 编辑器工具，为 Canvas 组件提供自定义图标和项目视图集成。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为 Canvas 预制体在项目窗口中显示自定义图标
    /// - 自动处理资源导入和移动事件
    /// - 支持在项目视图中快速识别 Canvas 对象
    /// 
    /// 使用手册
    /// 1. 编辑器集成
    /// 
    /// 1.1 自定义图标
    /// 
    ///     Canvas 预制体在项目窗口中会显示自定义图标，无需手动配置
    /// 
    /// 1.2 自动处理导入
    /// 
    ///     Canvas 预制体被导入或移动时，系统会自动应用自定义图标
    /// 
    /// </code>
    /// </remarks>
    public class UICanvasEditor
    {
        internal static Texture2D icon;

        /// <summary>
        /// 编辑器初始化方法，加载图标并注册项目窗口绘制回调。
        /// </summary>
        [InitializeOnLoadMethod]
        internal static void OnInit()
        {
            var pkg = XEditor.Utility.FindPackage();
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/{pkg.name}/Editor/Resources/Icon/Canvas.png");
            if (icon) EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        /// <summary>
        /// 项目窗口绘制回调，为 Canvas 预制体添加自定义图标。
        /// </summary>
        /// <param name="guid">资源的 GUID</param>
        /// <param name="selectionRect">项目窗口中的绘制区域</param>
        internal static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith("prefab") && AssetDatabase.LoadAssetAtPath<Canvas>(path))
            {
                Rect iconRect = new Rect(selectionRect.x + selectionRect.width - 20, selectionRect.y, 16, 16);
                GUI.DrawTexture(iconRect, icon);
            }
        }

        /// <summary>
        /// 资源后处理器，处理 Canvas 资源的导入和移动事件。
        /// </summary>
        internal class PostProcessor : AssetPostprocessor
        {
            /// <summary>
            /// 处理所有资源的后处理事件，为 Canvas 对象设置图标。
            /// </summary>
            /// <param name="importedAssets">导入的资源路径</param>
            /// <param name="deletedAssets">删除的资源路径</param>
            /// <param name="movedAssets">移动的资源路径</param>
            /// <param name="movedFromAssetPaths">资源移动前的路径</param>
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                var paths = new List<string>();
                paths.AddRange(importedAssets);
                paths.AddRange(movedAssets);
                foreach (var path in paths)
                {
                    if (path.EndsWith(".prefab"))
                    {
                        var canvas = AssetDatabase.LoadAssetAtPath<Canvas>(path);
                        if (canvas)
                        {
                            EditorGUIUtility.SetIconForObject(canvas.gameObject, icon);
                        }
                    }
                }
            }
        }
    }
}