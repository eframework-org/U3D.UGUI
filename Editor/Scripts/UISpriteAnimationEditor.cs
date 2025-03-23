// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using UnityEditor;
using UnityEngine;

namespace EFramework.UnityUI.Editor
{
    /// <summary>
    /// UISpriteAnimation 组件的自定义编辑器，提供精灵动画的编辑界面和创建功能。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为 UISpriteAnimation 组件提供自定义 Inspector 界面
    /// - 支持不同精灵模式的属性显示
    /// - 提供便捷的精灵动画创建菜单
    /// - 自动配置精灵动画的基本属性
    /// 
    /// 使用手册
    /// 1. 创建精灵动画
    /// 
    /// 1.1 通过菜单创建
    /// 
    ///     在 Hierarchy 窗口中右键选择 "UI/Sprite Animation"
    ///     系统将自动创建一个带有 UISpriteAnimation 组件的 UI 对象
    /// 
    /// 2. 配置精灵动画
    /// 
    /// 2.1 设置精灵模式
    /// 
    ///     选择 Atlas 模式使用图集进行动画
    ///     选择 Sprite 模式使用单独的精灵进行动画
    /// 
    /// </code>
    /// </remarks>
    [CustomEditor(typeof(UISpriteAnimation), true)]
    [CanEditMultipleObjects]
    public class UISpriteAnimationEditor : UnityEditor.Editor
    {
        /// <summary>
        /// 精灵模式属性
        /// </summary>
        private SerializedProperty spriteModeProp;

        /// <summary>
        /// 图集属性
        /// </summary>
        private SerializedProperty atlasProp;

        /// <summary>
        /// 精灵前缀属性
        /// </summary>
        private SerializedProperty spritesProp;

        /// <summary>
        /// 帧率属性
        /// </summary>
        private SerializedProperty frameRateProp;

        /// <summary>
        /// 循环播放属性
        /// </summary>
        private SerializedProperty loopProp;

        /// <summary>
        /// 原始尺寸属性
        /// </summary>
        private SerializedProperty nativeSizeProp;

        /// <summary>
        /// 启用编辑器时初始化序列化属性。
        /// </summary>
        protected virtual void OnEnable()
        {
            spriteModeProp = serializedObject.FindProperty("spriteMode");
            atlasProp = serializedObject.FindProperty("sprites");
            spritesProp = serializedObject.FindProperty("prefix");
            frameRateProp = serializedObject.FindProperty("frameRate");
            loopProp = serializedObject.FindProperty("loop");
            nativeSizeProp = serializedObject.FindProperty("nativeSize");
        }

        /// <summary>
        /// 绘制自定义 Inspector 界面。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(spriteModeProp, new GUIContent("SpriteMode"));

            if (spriteModeProp.enumValueFlag == (int)UISpriteAnimation.SpriteMode.Atlas)
            {
                EditorGUILayout.PropertyField(atlasProp);
            }
            else if (spriteModeProp.enumValueFlag == (int)UISpriteAnimation.SpriteMode.Sprite)
            {
                EditorGUILayout.PropertyField(spritesProp);
            }

            EditorGUILayout.PropertyField(loopProp);
            EditorGUILayout.PropertyField(nativeSizeProp);
            EditorGUILayout.PropertyField(frameRateProp);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 创建精灵动画的菜单项处理方法。
        /// </summary>
        [MenuItem("GameObject/UI/Sprite Animation")]
        private static void CreateTemplate()
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            var selected = Selection.activeGameObject;
            selected.name = "Sprite Animation";
            var spriteAnimation = selected.AddComponent<UISpriteAnimation>();
            var serialObj = new SerializedObject(spriteAnimation);
            serialObj.ApplyModifiedProperties();
            Selection.activeGameObject = selected;
            Undo.RegisterCreatedObjectUndo(selected, "Create Sprite Animation");
        }
    }
}
