// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace EFramework.UnityUI.Editor
{
    /// <summary>
    /// UIWrapContent 组件的自定义编辑器，提供包装内容布局的编辑界面和创建功能。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为 UIWrapContent 组件提供自定义 Inspector 界面
    /// - 支持垂直列表、水平列表、垂直网格和水平网格四种布局模式
    /// - 提供便捷的Unity编辑器菜单创建组件
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [CustomEditor(typeof(UIWrapContent), true)]
    [CanEditMultipleObjects]
    public class UIWrapContentEditor : ScrollRectEditor
    {
        /// <summary>
        /// 当前编辑的 UIWrapContent 实例
        /// </summary>
        private UIWrapContent activeTarget;

        /// <summary>
        /// 布局模式属性
        /// </summary>
        private SerializedProperty layoutProp;

        /// <summary>
        /// 分段属性（列表模式下为间距，网格模式下为列数或行数）
        /// </summary>
        private SerializedProperty segmentProp;

        /// <summary>
        /// 边距属性（列表模式下为边距，网格模式下为间距）
        /// </summary>
        private SerializedProperty paddingProp;

        /// <summary>
        /// 启用编辑器时初始化序列化属性。
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            activeTarget = (UIWrapContent)target;
            layoutProp = serializedObject.FindProperty("layout");
            segmentProp = serializedObject.FindProperty("segment");
            paddingProp = serializedObject.FindProperty("padding");
        }

        /// <summary>
        /// 绘制自定义 Inspector 界面，根据不同的布局模式显示不同的属性。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(layoutProp, new GUIContent("Layout"));
            switch (activeTarget.Layout)
            {
                case UIWrapContent.LayoutMode.VerticalList:
                    EditorGUILayout.PropertyField(segmentProp, new GUIContent("Space"));
                    EditorGUILayout.PropertyField(paddingProp, new GUIContent("Margin"));
                    EditorGUILayout.HelpBox("X - Top, Y - Bottom", MessageType.Info, false);
                    break;
                case UIWrapContent.LayoutMode.HorizontalList:
                    EditorGUILayout.PropertyField(segmentProp, new GUIContent("Space"));
                    EditorGUILayout.PropertyField(paddingProp, new GUIContent("Margin"));
                    EditorGUILayout.HelpBox("X - Left, Y - Right", MessageType.Info, false);
                    break;
                case UIWrapContent.LayoutMode.VerticalGrid:
                    EditorGUILayout.PropertyField(paddingProp, new GUIContent("Space"));
                    EditorGUILayout.HelpBox("X - Horizontal, Y - Vertical", MessageType.Info, false);
                    if (EditorGUILayout.PropertyField(segmentProp, new GUIContent("Columns"))) segmentProp.intValue = Math.Max(1, segmentProp.intValue);
                    break;
                case UIWrapContent.LayoutMode.HorizontalGrid:
                    EditorGUILayout.PropertyField(paddingProp, new GUIContent("Space"));
                    EditorGUILayout.HelpBox("X - Horizontal, Y - Vertical", MessageType.Info, false);
                    if (EditorGUILayout.PropertyField(segmentProp, new GUIContent("Rows"))) segmentProp.intValue = Math.Max(1, segmentProp.intValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        /// <summary>
        /// 创建包装内容的菜单项处理方法，基于滚动视图添加 UIWrapContent 组件。
        /// </summary>
        [MenuItem("GameObject/UI/Wrap Content")]
        private static void CreateTemplate()
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Scroll View");
            var selected = Selection.activeGameObject;
            selected.name = "Wrap Content";
            var scrollRect = selected.GetComponent<ScrollRect>();
            var serialObj = new SerializedObject(scrollRect);
            List<SerializedProperty> serialProps = new List<SerializedProperty>();
            SerializedProperty it = serialObj.GetIterator();
            bool children = true;
            while (it.NextVisible(children))
            {
                if (it.name == "m_Script") continue;
                serialProps.Add(it.Copy());
            }
            DestroyImmediate(scrollRect);
            var wrapContent = selected.AddComponent<UIWrapContent>();
            serialObj = new SerializedObject(wrapContent);
            foreach (var item in serialProps)
            {
                serialObj.CopyFromSerializedProperty(item);
            }
            serialObj.ApplyModifiedProperties();
            Selection.activeGameObject = selected;
            Undo.RegisterCreatedObjectUndo(selected, "Create Wrap Content");
        }
    }
}
