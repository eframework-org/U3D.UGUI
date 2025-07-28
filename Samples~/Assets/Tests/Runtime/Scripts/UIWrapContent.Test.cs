// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class TestUIWrapContent
{
    private GameObject canvasObj;
    private UIWrapContent wrapContent;
    private GameObject itemTemplate;
    private RectTransform contentRect;
    private RectTransform viewportRect;

    [SetUp]
    public void Setup()
    {
        // 创建Canvas
        canvasObj = new GameObject("TestCanvas");
        canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.SetActive(false);

        // 创建WrapContent容器
        var wrapContentObj = new GameObject("WrapContent", typeof(RectTransform));
        wrapContentObj.transform.SetParent(canvasObj.transform);
        wrapContent = wrapContentObj.AddComponent<UIWrapContent>();

        // 创建Viewport
        var viewportObj = new GameObject("Viewport", typeof(RectTransform));
        viewportRect = viewportObj.GetComponent<RectTransform>();
        viewportObj.transform.SetParent(wrapContentObj.transform);
        viewportObj.AddComponent<RectMask2D>();

        // 创建Content
        var contentObj = new GameObject("Content", typeof(RectTransform));
        contentObj.transform.SetParent(viewportObj.transform);
        contentRect = contentObj.GetComponent<RectTransform>();

        // 创建单元格模板
        itemTemplate = new GameObject("ItemTemplate", typeof(RectTransform));
        itemTemplate.transform.SetParent(contentRect);
        itemTemplate.AddComponent<Image>();

        // 配置UIWrapContent
        wrapContent.content = contentRect;
        wrapContent.viewport = viewportRect;
    }

    [TearDown]
    public void Reset()
    {
        // 清理测试对象
        Object.DestroyImmediate(canvasObj);
    }

    [UnityTest]
    public IEnumerator SimpleAdapter()
    {
        // 设置垂直列表布局
        wrapContent.Layout = UIWrapContent.LayoutMode.VerticalList;

        // 测试数据
        var items = new List<string> { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9", "Item 10" };
        int setCount = 0;

        // 创建简单适配器
        wrapContent.Adapter = new UIWrapContent.ISimpleAdapter(
            wrapContent,
            () => items.Count, // 获取总数函数
            (cell, index) =>
            {
                Debug.Log($"设置单元格数据: {index}, {setCount}, {items.Count}");
                setCount++;
            }
        );
        canvasObj.SetActive(true);

        // 等待初始化
        yield return new WaitForSeconds(0.1f);

        // 验证初始化和首次加载
        Assert.IsTrue(wrapContent.Inited, "WrapContent应该初始化成功");
        Assert.Greater(setCount, 0, "至少应该设置一个单元格");
        Assert.Less(setCount, items.Count, "由于复用，不应该设置所有单元格");

        // 滚动到中间位置
        int prevSetCount = setCount;
        wrapContent.ScrollTo(0.5f);

        // 等待滚动
        yield return new WaitForSeconds(0.1f);

        // 验证滚动加载
        Assert.Greater(setCount, prevSetCount, "滚动后应该有更多单元格被设置");

        // 测试EachItem方法
        int eachCount = 0;
        wrapContent.EachItem((index, item) =>
        {
            eachCount++;
        });

        Assert.Greater(eachCount, 0, "EachItem应该能够遍历所有显示的单元格");
    }

    [UnityTest]
    public IEnumerator CustomAdapter()
    {
        // 配置水平列表
        wrapContent.Layout = UIWrapContent.LayoutMode.HorizontalList;
        wrapContent.horizontal = true;
        wrapContent.vertical = false;

        // 实现自定义适配器
        var customAdapter = new MyAdapter(10);
        wrapContent.Adapter = customAdapter;
        canvasObj.SetActive(true);

        // 等待初始化
        yield return new WaitForSeconds(0.1f);

        // 验证适配器和初始化
        Assert.IsTrue(wrapContent.Inited, "WrapContent应该初始化成功");
        Assert.Greater(customAdapter.SetCellCount, 0, "应该有单元格被设置");

        // 滚动到末尾
        wrapContent.ScrollTo(1.0f);

        // 等待滚动
        yield return new WaitForSeconds(0.1f);

        // 验证滚动加载
        int setCountAfterScroll = customAdapter.SetCellCount;
        Assert.Greater(setCountAfterScroll, 0, "滚动后应该有单元格被设置");

        // 改变数据数量并重新加载
        customAdapter.ItemCount = 5;
        wrapContent.Reload();

        // 等待重载
        yield return new WaitForSeconds(0.1f);

        // 验证重载后状态
        customAdapter.SetCellCount = 0; // 重置计数
        wrapContent.EachItem((index, item) =>
        {
            Assert.Less(index, 5, "索引不应超过新的数据数量");
        });
    }

    /// <summary>
    /// 测试用自定义适配器
    /// </summary>
    private class MyAdapter : UIWrapContent.ICustomAdapter
    {
        public int ItemCount { get; set; }
        public int SetCellCount { get; set; }

        public MyAdapter(int itemCount)
        {
            ItemCount = itemCount;
            SetCellCount = 0;
        }

        public bool SingleZygote => true;

        public int GetCellType(int index) => 0;

        public RectTransform GetCellZygote(int type) => Object.FindObjectOfType<UIWrapContent>().content.GetChild(0).GetComponent<RectTransform>();

        public int GetCellCount() => ItemCount;

        public void SetCellData(RectTransform cell, int index)
        {
            SetCellCount++;
        }
    }
}
#endif
