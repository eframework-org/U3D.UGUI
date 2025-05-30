// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UIEventListener 是一个 UI 事件监听器组件，封装了多种 Unity UI 事件接口，用于处理 UI 交互的各种事件。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - Unity UI 事件系统接口封装
    /// - 支持事件回调和事件委托绑定
    /// 
    /// 使用手册
    /// 1. 组件获取
    /// 
    ///     1. 获取或添加监听器：
    ///       - 获取GameObject上的监听器，如果不存在则自动添加：
    /// 
    ///         UIEventListener listener = UIEventListener.Get(gameObject);
    /// 
    ///       - 也可以传入组件来获取其GameObject上的监听器：
    /// 
    ///         UIEventListener listener = UIEventListener.Get(transform);
    /// 
    /// 2. 事件绑定
    /// 
    ///     1. 使用事件委托：
    /// 
    ///         // 使用事件委托绑定点击事件
    ///         listener.OnPointerClickEvent += (eventData) => {
    ///             Debug.Log("点击了：" + gameObject.name);
    ///         };
    /// 
    ///     2. 使用回调函数：
    /// 
    ///         // 直接指定回调函数
    ///         listener.OnPointerClickFunc = HandleClick;
    ///         
    ///         // 回调函数定义
    ///         private void HandleClick(PointerEventData eventData) {
    ///             Debug.Log("点击了：" + eventData.pointerCurrentRaycast.gameObject.name);
    ///         }
    /// 
    /// 常见问题
    /// 1. 如何判断一个 UI 元素当前是否被按下？
    /// 
    ///     UIEventListener组件包含一个IsPressed属性，可以用来检查UI元素当前的按压状态：
    /// 
    ///     UIEventListener listener = UIEventListener.Get(buttonObj);
    ///     if (listener.IsPressed) {
    ///         // 按钮当前被按下
    ///         Debug.Log("按钮处于按下状态");
    ///     }
    /// 
    /// 2. UIEventListener 支持哪些类型的事件？
    /// 
    ///     UIEventListener支持以下类型的事件：
    ///       - 按钮选中/取消选中 (OnButtonSelectEvent/OnButtonDeselectEvent)
    ///       - 指针点击 (OnPointerClickEvent)
    ///       - 指针按下/抬起 (OnPointerDownEvent/OnPointerUpEvent)
    ///       - 指针进入/退出 (OnPointerEnterEvent/OnPointerExitEvent)
    ///       - 拖拽相关 (OnBeginDragEvent/OnDragEvent/OnEndDragEvent)
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [AddComponentMenu("UI/Event Listener")]
    public class UIEventListener : MonoBehaviour, ISelectHandler, IDeselectHandler,
        IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 指针事件委托，用于处理与指针相关的事件回调。
        /// 包括点击、按下、进入、退出、抬起以及拖拽相关事件。
        /// </summary>
        /// <param name="data">指针事件数据，包含事件发生时的指针信息</param>
        public delegate void PointEventDelegate(PointerEventData data);

        /// <summary>
        /// 基础事件委托，用于处理基础UI事件回调。
        /// 包括选中和取消选中等非指针相关事件。
        /// </summary>
        /// <param name="data">基础事件数据，包含事件相关信息</param>
        public delegate void BaseEventDelegate(BaseEventData data);

        /// <summary>
        /// 按钮选中事件。
        /// 当UI元素被选中时触发，可通过添加事件监听器响应。
        /// </summary>
        public event BaseEventDelegate OnButtonSelectEvent;

        /// <summary>
        /// 按钮选中回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public BaseEventDelegate OnButtonSelectFunc;

        /// <summary>
        /// 按钮取消选中事件。
        /// 当UI元素失去选中状态时触发，可通过添加事件监听器响应。
        /// </summary>
        public event BaseEventDelegate OnButtonDeselectEvent;

        /// <summary>
        /// 按钮取消选中回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public BaseEventDelegate OnButtonDeselectFunc;

        /// <summary>
        /// 指针点击事件。
        /// 当UI元素被点击时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnPointerClickEvent;

        /// <summary>
        /// 指针点击回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnPointerClickFunc;

        /// <summary>
        /// 指针按下事件。
        /// 当指针在UI元素上按下时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnPointerDownEvent;

        /// <summary>
        /// 指针按下回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnPointerDownFunc;

        /// <summary>
        /// 指针进入事件。
        /// 当指针进入UI元素区域时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnPointerEnterEvent;

        /// <summary>
        /// 指针进入回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnPointerEnterFunc;

        /// <summary>
        /// 指针退出事件。
        /// 当指针离开UI元素区域时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnPointerExitEvent;

        /// <summary>
        /// 指针退出回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnPointerExitFunc;

        /// <summary>
        /// 指针抬起事件。
        /// 当指针在UI元素上抬起时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnPointerUpEvent;

        /// <summary>
        /// 指针抬起回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnPointerUpFunc;

        /// <summary>
        /// 开始拖拽事件。
        /// 当在UI元素上开始拖拽操作时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnBeginDragEvent;

        /// <summary>
        /// 开始拖拽回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnBeginDragFunc;

        /// <summary>
        /// 拖拽中事件。
        /// 当正在拖拽UI元素时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnDragEvent;

        /// <summary>
        /// 拖拽中回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnDragFunc;

        /// <summary>
        /// 结束拖拽事件。
        /// 当结束拖拽UI元素时触发，可通过添加事件监听器响应。
        /// </summary>
        public event PointEventDelegate OnEndDragEvent;

        /// <summary>
        /// 结束拖拽回调函数。
        /// 可直接设置此属性指定回调，作为事件的替代方式。
        /// </summary>
        public PointEventDelegate OnEndDragFunc;

        /// <summary>
        /// 按钮是否处于按下状态。
        /// 用于跟踪按钮的当前按压状态，可在外部代码中检查此状态。
        /// </summary>
        [NonSerialized] public bool IsPressed;

        /// <summary>
        /// 处理UI元素失去选中状态的事件。
        /// 当UI元素失去选中状态时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">事件数据，包含选中相关信息</param>
        public void OnDeselect(BaseEventData data) { OnButtonDeselectEvent?.Invoke(data); OnButtonDeselectFunc?.Invoke(data); }

        /// <summary>
        /// 处理UI元素被选中的事件。
        /// 当UI元素被选中时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">事件数据，包含选中相关信息</param>
        public void OnSelect(BaseEventData data) { OnButtonSelectEvent?.Invoke(data); OnButtonSelectFunc?.Invoke(data); }

        /// <summary>
        /// 处理指针点击事件。
        /// 当指针点击UI元素时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含点击相关信息</param>
        void IPointerClickHandler.OnPointerClick(PointerEventData data) { OnPointerClickEvent?.Invoke(data); OnPointerClickFunc?.Invoke(data); }

        /// <summary>
        /// 处理指针按下事件。
        /// 当指针在UI元素上按下时，设置IsPressed状态并调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含按下相关信息</param>
        void IPointerDownHandler.OnPointerDown(PointerEventData data) { IsPressed = true; OnPointerDownEvent?.Invoke(data); OnPointerDownFunc?.Invoke(data); }

        /// <summary>
        /// 处理指针进入UI元素区域的事件。
        /// 当指针进入UI元素区域时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含进入相关信息</param>
        void IPointerEnterHandler.OnPointerEnter(PointerEventData data) { OnPointerEnterEvent?.Invoke(data); OnPointerEnterFunc?.Invoke(data); }

        /// <summary>
        /// 处理指针离开UI元素区域的事件。
        /// 当指针离开UI元素区域时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含离开相关信息</param>
        void IPointerExitHandler.OnPointerExit(PointerEventData data) { OnPointerExitEvent?.Invoke(data); OnPointerExitFunc?.Invoke(data); }

        /// <summary>
        /// 处理指针抬起事件。
        /// 当指针在UI元素上抬起时，重置IsPressed状态并调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含抬起相关信息</param>
        void IPointerUpHandler.OnPointerUp(PointerEventData data) { IsPressed = false; OnPointerUpEvent?.Invoke(data); OnPointerUpFunc?.Invoke(data); }

        /// <summary>
        /// 处理开始拖拽事件。
        /// 当在UI元素上开始拖拽操作时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含拖拽相关信息</param>
        void IBeginDragHandler.OnBeginDrag(PointerEventData data) { OnBeginDragEvent?.Invoke(data); OnBeginDragFunc?.Invoke(data); }

        /// <summary>
        /// 处理拖拽过程中的事件。
        /// 当正在拖拽UI元素时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含拖拽相关信息</param>
        void IDragHandler.OnDrag(PointerEventData data) { OnDragEvent?.Invoke(data); OnDragFunc?.Invoke(data); }

        /// <summary>
        /// 处理结束拖拽事件。
        /// 当结束拖拽UI元素时，调用相应的事件委托和回调函数。
        /// </summary>
        /// <param name="data">指针事件数据，包含拖拽相关信息</param>
        void IEndDragHandler.OnEndDrag(PointerEventData data) { OnEndDragEvent?.Invoke(data); OnEndDragFunc?.Invoke(data); }

        /// <summary>
        /// 获取指定组件GameObject上的UIEventListener组件，若不存在则自动添加。
        /// 简化获取和添加监听器的过程。
        /// </summary>
        /// <param name="comp">目标组件</param>
        /// <returns>UIEventListener实例，如果组件为null则返回null</returns>
        public static UIEventListener Get(Component comp)
        {
            if (comp) return Get(comp.gameObject);
            return null;
        }

        /// <summary>
        /// 获取指定GameObject上的UIEventListener组件，若不存在则自动添加。
        /// 简化获取和添加监听器的过程。
        /// </summary>
        /// <param name="obj">目标游戏对象</param>
        /// <returns>UIEventListener实例</returns>
        public static UIEventListener Get(GameObject obj)
        {
            var listener = obj.GetComponent<UIEventListener>();
            if (listener == null) listener = obj.AddComponent<UIEventListener>();
            return listener;
        }
    }
}
