// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UIButtonScale 实现了按钮缩放的交互效果，支持按钮按下时自动缩放，松开时恢复原始大小，提供了良好的视觉反馈。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 自定义缩放比例和过渡效果
    /// - 自动处理按下和抬起事件
    /// 
    /// 使用手册
    /// 1. 基本使用
    /// 
    ///     1. 添加组件：
    ///       - 选择需要添加缩放效果的按钮对象
    ///       - 在 Inspector 窗口点击 "Add Component"
    ///       - 选择 "UI/Button Scale" 或搜索 "UIButtonScale"
    /// 
    ///     2. 组件配置：
    ///       - Button：按钮组件引用，默认自动获取当前对象上的 Button 组件
    ///       - Scale：按下时的缩放比例，默认为 0.95
    /// 
    /// 2. 实现原理
    /// 
    ///     组件会自动监听按钮的指针事件：
    ///       - 当按下时，按钮会缩小到设定的比例
    ///       - 当指针抬起时，按钮会恢复到原始大小
    ///       - 缩放效果是即时的，没有过渡动画
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [AddComponentMenu("UI/Button Scale")]
    public class UIButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 需要应用缩放效果的按钮组件。
        /// 如果未指定，将尝试从当前GameObject获取。
        /// </summary>
        public Button button;

        /// <summary>
        /// 按钮的初始缩放值，用于在交互后恢复。
        /// </summary>
        private Vector3 initialScale;

        /// <summary>
        /// 在组件唤醒时初始化按钮引用和初始缩放值。
        /// </summary>
        private void Awake()
        {
            if (button == null) button = transform.GetComponent<Button>();
            if (button != null)
            {
                initialScale = button.transform.localScale;
            }
        }

        /// <summary>
        /// 在组件启用时确保按钮恢复到初始缩放值。
        /// </summary>
        private void OnEnable()
        {
            if (button != null && initialScale != null)
            {
                button.transform.localScale = initialScale;
            }
        }

        /// <summary>
        /// 处理指针按下事件，使按钮缩小到原始大小的95%。
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (button == null) button = transform.GetComponent<Button>();
            if (button != null)
            {
                button.transform.localScale = initialScale * 0.95f;
            }
        }

        /// <summary>
        /// 处理指针释放事件，使按钮恢复到初始大小。
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (button == null) button = transform.GetComponent<Button>();
            if (button != null)
            {
                button.transform.localScale = initialScale;
            }
        }
    }
}
