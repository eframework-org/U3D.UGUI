// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestUIEventListener
{
    [Test]
    public void Get()
    {
        // 创建测试对象
        var obj = new GameObject("TestObject");

        // 当对象上不存在UIEventListener时调用Get
        var listener = UIEventListener.Get(obj);
        Assert.IsNotNull(listener, "应返回UIEventListener实例");
        Assert.AreEqual(obj, listener.gameObject, "应将组件添加到正确的GameObject上");

        // 当对象上存在UIEventListener时，直接返回已存在的实例
        var listenerAgain = UIEventListener.Get(obj);
        Assert.AreSame(listener, listenerAgain, "重复获取应返回已存在的实例");

        // 清理
        Object.DestroyImmediate(obj);
    }

    [Test]
    public void Event()
    {
        // 创建测试对象
        var gameObject = new GameObject("TestObject");
        var listener = gameObject.AddComponent<UIEventListener>();

        // 创建测试事件数据
        var eventData = new BaseEventData(null);
        var pointerEventData = new PointerEventData(null);

        // 测试变量
        bool selectCalled = false;
        bool deselectCalled = false;
        bool clickCalled = false;
        bool downCalled = false;
        bool enterCalled = false;
        bool exitCalled = false;
        bool upCalled = false;
        bool beginDragCalled = false;
        bool dragCalled = false;
        bool endDragCalled = false;

        // 注册事件处理器
        listener.OnButtonSelectEvent += (data) => { selectCalled = true; };
        listener.OnButtonDeselectEvent += (data) => { deselectCalled = true; };
        listener.OnPointerClickEvent += (data) => { clickCalled = true; };
        listener.OnPointerDownEvent += (data) => { downCalled = true; };
        listener.OnPointerEnterEvent += (data) => { enterCalled = true; };
        listener.OnPointerExitEvent += (data) => { exitCalled = true; };
        listener.OnPointerUpEvent += (data) => { upCalled = true; };
        listener.OnBeginDragEvent += (data) => { beginDragCalled = true; };
        listener.OnDragEvent += (data) => { dragCalled = true; };
        listener.OnEndDragEvent += (data) => { endDragCalled = true; };

        // 触发事件
        listener.OnSelect(eventData);
        listener.OnDeselect(eventData);
        ((IPointerClickHandler)listener).OnPointerClick(pointerEventData);
        ((IPointerDownHandler)listener).OnPointerDown(pointerEventData);
        ((IPointerEnterHandler)listener).OnPointerEnter(pointerEventData);
        ((IPointerExitHandler)listener).OnPointerExit(pointerEventData);
        ((IPointerUpHandler)listener).OnPointerUp(pointerEventData);
        ((IBeginDragHandler)listener).OnBeginDrag(pointerEventData);
        ((IDragHandler)listener).OnDrag(pointerEventData);
        ((IEndDragHandler)listener).OnEndDrag(pointerEventData);

        // 验证所有事件都被正确触发
        Assert.IsTrue(selectCalled, "OnButtonSelectEvent 应被触发");
        Assert.IsTrue(deselectCalled, "OnButtonDeselectEvent 应被触发");
        Assert.IsTrue(clickCalled, "OnPointerClickEvent 应被触发");
        Assert.IsTrue(downCalled, "OnPointerDownEvent 应被触发");
        Assert.IsTrue(enterCalled, "OnPointerEnterEvent 应被触发");
        Assert.IsTrue(exitCalled, "OnPointerExitEvent 应被触发");
        Assert.IsTrue(upCalled, "OnPointerUpEvent 应被触发");
        Assert.IsTrue(beginDragCalled, "OnBeginDragEvent 应被触发");
        Assert.IsTrue(dragCalled, "OnDragEvent 应被触发");
        Assert.IsTrue(endDragCalled, "OnEndDragEvent 应被触发");

        // 清理
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void Func()
    {
        // 创建测试对象
        var gameObject = new GameObject("TestObject");
        var listener = gameObject.AddComponent<UIEventListener>();

        // 创建测试事件数据
        var eventData = new BaseEventData(null);
        var pointerEventData = new PointerEventData(null);

        // 测试变量
        bool selectCalled = false;
        bool deselectCalled = false;
        bool clickCalled = false;
        bool downCalled = false;
        bool enterCalled = false;
        bool exitCalled = false;
        bool upCalled = false;
        bool beginDragCalled = false;
        bool dragCalled = false;
        bool endDragCalled = false;

        // 设置函数回调
        listener.OnButtonSelectFunc = (data) => { selectCalled = true; };
        listener.OnButtonDeselectFunc = (data) => { deselectCalled = true; };
        listener.OnPointerClickFunc = (data) => { clickCalled = true; };
        listener.OnPointerDownFunc = (data) => { downCalled = true; };
        listener.OnPointerEnterFunc = (data) => { enterCalled = true; };
        listener.OnPointerExitFunc = (data) => { exitCalled = true; };
        listener.OnPointerUpFunc = (data) => { upCalled = true; };
        listener.OnBeginDragFunc = (data) => { beginDragCalled = true; };
        listener.OnDragFunc = (data) => { dragCalled = true; };
        listener.OnEndDragFunc = (data) => { endDragCalled = true; };

        // 触发事件
        listener.OnSelect(eventData);
        listener.OnDeselect(eventData);
        ((IPointerClickHandler)listener).OnPointerClick(pointerEventData);
        ((IPointerDownHandler)listener).OnPointerDown(pointerEventData);
        ((IPointerEnterHandler)listener).OnPointerEnter(pointerEventData);
        ((IPointerExitHandler)listener).OnPointerExit(pointerEventData);
        ((IPointerUpHandler)listener).OnPointerUp(pointerEventData);
        ((IBeginDragHandler)listener).OnBeginDrag(pointerEventData);
        ((IDragHandler)listener).OnDrag(pointerEventData);
        ((IEndDragHandler)listener).OnEndDrag(pointerEventData);

        // 验证所有函数回调都被正确触发
        Assert.IsTrue(selectCalled, "OnButtonSelectFunc 应被触发");
        Assert.IsTrue(deselectCalled, "OnButtonDeselectFunc 应被触发");
        Assert.IsTrue(clickCalled, "OnPointerClickFunc 应被触发");
        Assert.IsTrue(downCalled, "OnPointerDownFunc 应被触发");
        Assert.IsTrue(enterCalled, "OnPointerEnterFunc 应被触发");
        Assert.IsTrue(exitCalled, "OnPointerExitFunc 应被触发");
        Assert.IsTrue(upCalled, "OnPointerUpFunc 应被触发");
        Assert.IsTrue(beginDragCalled, "OnBeginDragFunc 应被触发");
        Assert.IsTrue(dragCalled, "OnDragFunc 应被触发");
        Assert.IsTrue(endDragCalled, "OnEndDragFunc 应被触发");

        // 清理
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void Mixed()
    {
        // 创建测试对象
        var gameObject = new GameObject("TestObject");
        var listener = gameObject.AddComponent<UIEventListener>();
        var pointerEventData = new PointerEventData(null);
        // 计数器
        int clickCount = 0;

        // 注册多个事件处理器
        listener.OnPointerClickEvent += (data) => { clickCount++; };
        listener.OnPointerClickFunc = (data) => { clickCount++; };

        // 验证所有处理器都被调用
        ((IPointerClickHandler)listener).OnPointerClick(pointerEventData);
        Assert.AreEqual(2, clickCount, "所有注册的处理器应被调用");

        // 清理
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void Pressed()
    {
        // 创建测试对象
        var gameObject = new GameObject("TestObject");
        var listener = gameObject.AddComponent<UIEventListener>();
        var pointerEventData = new PointerEventData(null);

        // 初始状态
        Assert.IsFalse(listener.IsPressed, "初始IsPressed应为false");

        // 触发按下事件
        ((IPointerDownHandler)listener).OnPointerDown(pointerEventData);
        Assert.IsTrue(listener.IsPressed, "按下后IsPressed应为true");

        // 触发抬起事件
        ((IPointerUpHandler)listener).OnPointerUp(pointerEventData);
        Assert.IsFalse(listener.IsPressed, "抬起后IsPressed应恢复为false");

        // 清理
        Object.DestroyImmediate(gameObject);
    }
}
#endif
