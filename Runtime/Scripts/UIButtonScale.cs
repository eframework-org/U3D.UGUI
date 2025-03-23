// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// 按钮缩放组件。
    /// 为UI按钮添加按下时的缩放效果，提升交互反馈。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为按钮添加按下时的缩放视觉反馈
    /// - 自动处理指针按下和释放事件
    /// - 支持自动查找和引用按钮组件
    /// - 无需编写任何代码即可实现按钮动效
    /// 
    /// 使用手册
    /// 1. 添加组件
    /// 
    /// 1.1 添加到按钮对象
    /// 
    ///     选择含有 Button 组件的游戏对象
    ///     添加 UIButtonScale 组件
    ///     组件会自动查找并引用 Button
    /// 
    /// 1.2 自定义按钮引用
    /// 
    ///     如果需要对其他按钮应用效果
    ///     可以手动指定 button 引用
    /// 
    /// 2. 运行时行为
    /// 
    /// 2.1 按下效果
    /// 
    ///     按下按钮时，按钮将缩小到原始大小的 95%
    ///     释放按钮时，按钮将恢复原始大小
    /// 
    /// </code>
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
