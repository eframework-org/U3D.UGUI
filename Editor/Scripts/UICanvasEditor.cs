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
    /// UICanvasEditor 是 UICanvas 组件的编辑器工具，提供了自定义图标和项目视图集成。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为 Canvas 预制体在项目窗口中显示自定义图标
    /// - 自动处理资源导入和移动事件
    /// - 支持在项目视图中快速识别 Canvas 对象
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    public class UICanvasEditor : XEditor.Event.Internal.OnEditorLoad
    {
        /// <summary>
        /// icon 是 UICanvas 组件的自定义图标，用于优化资源的显示。
        /// </summary>
        internal static Texture2D icon;

        int XEditor.Event.Callback.Priority { get; }

        bool XEditor.Event.Callback.Singleton { get => true; }

        /// <summary>
        /// OnEditorLoad 事件回调处理了文件视图的绘制监听。
        /// </summary>
        void XEditor.Event.Internal.OnEditorLoad.Process(params object[] _)
        {
            // 加载图标
            var pkg = XEditor.Utility.FindPackage();
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/{pkg.name}/Editor/Resources/Icon/Canvas.png");

            // 监听绘制
            if (icon) EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        /// <summary>
        /// OnProjectWindowItemOnGUI 是项目窗口绘制的回调，为 UICanvas 预制体添加自定义图标。
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
        /// PostProcessor 是资源后处理器，处理 Canvas 资源的导入和移动事件。
        /// </summary>
        internal class PostProcessor : AssetPostprocessor
        {
            /// <summary>
            /// OnPostprocessAllAssets 处理所有资源的后处理事件，为 Canvas 对象设置图标。
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
