// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UIWrapContent 是一个拓展自 ScrollRect 的循环列表视图组件，提供了高效的大数据列表和网格显示能力，通过元素复用降低内存占用。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 多种布局模式：支持垂直列表、水平列表、垂直网格、水平网格四种布局
    /// - 元素复用机制：支持元素复用，只创建可见区域所需的 UI 单元格元素
    /// - 适配器支持：提供灵活的适配器接口，便于自定义数据绑定逻辑
    /// 
    /// 使用手册
    /// 1. 适配器实现
    /// 
    ///     1. 简单适配器：
    /// 
    ///         // 创建简单适配器
    ///         wrapContent.Adapter = new UIWrapContent.ISimpleAdapter(
    ///             wrapContent,
    ///             () => dataList.Count, // 获取数据总数
    ///             (cell, index) => {
    ///                 // 设置单元格数据
    ///                 cell.GetComponent&lt;Text&gt;().text = dataList[index].ToString();
    ///             }
    ///         );
    /// 
    ///     2. 自定义适配器：
    /// 
    ///         // 创建自定义适配器类
    ///         public class MyAdapter : UIWrapContent.ICustomAdapter 
    ///         {
    ///             private List&lt;ItemData&gt; dataList;
    ///             
    ///             public MyAdapter(List&lt;ItemData&gt; dataList) {
    ///                 this.dataList = dataList;
    ///             }
    ///             
    ///             public bool SingleZygote => false; // 多种单元格类型
    ///             
    ///             public int GetCellType(int index) {
    ///                 return dataList[index].type; // 根据数据类型返回
    ///             }
    ///             
    ///             public RectTransform GetCellZygote(int type) => Object.FindAnyObjectByType&lt;UIWrapContent&gt;().content.GetChild(0).GetComponent&lt;RectTransform&gt;();
    ///             
    ///             public int GetCellCount() {
    ///                 return dataList.Count;
    ///             }
    ///             
    ///             public void SetCellData(RectTransform cell, int index) {
    ///                 var data = dataList[index];
    ///                 // 根据数据更新UI元素
    ///                 if (GetCellType(index) == 0) {
    ///                     // 设置类型A单元格的数据
    ///                     var nameText = cell.Find("Name").GetComponent&lt;Text&gt;();
    ///                     nameText.text = data.name;
    ///                 } else {
    ///                     // 设置类型B单元格的数据
    ///                     var valueText = cell.Find("Value").GetComponent&lt;Text&gt;();
    ///                     valueText.text = data.value.ToString();
    ///                 }
    ///             }
    ///         }
    ///         
    ///         // 使用自定义适配器
    ///         wrapContent.Adapter = new MyAdapter(dataList);
    /// 
    /// 2. 运行时控制
    /// 
    ///     1. 滚动控制：
    /// 
    ///         // 滚动到指定位置（0-1范围）
    ///         wrapContent.ScrollTo(0.5f); // 滚动到中间位置
    ///         wrapContent.ScrollTo(0f);   // 滚动到开头
    ///         wrapContent.ScrollTo(1f);   // 滚动到末尾
    /// 
    ///     2. 数据更新：
    /// 
    ///         // 重新加载列表
    ///         wrapContent.Reload();
    /// 
    ///     3. 遍历可见项：
    /// 
    ///         // 遍历当前可见的所有单元格
    ///         wrapContent.EachItem((index, item) => {
    ///             Debug.Log($"可见项: 索引={index}, 名称={item.name}");
    ///         });
    /// 
    /// 常见问题
    /// 1. UIWrapContent 组件与 ScrollRect 组件有什么区别？
    /// 
    ///     ScrollRect 会为每个数据项创建一个 UI 单元格元素，当数据量大时会导致性能问题和内存占用过高。
    ///     UIWrapContent 采用元素复用机制，只创建可见区域所需的 UI 单元格元素，特别适合大数据列表。
    /// 
    /// 2. 如何优化 UIWrapContent 组件的性能？
    /// 
    ///     以下是一些优化建议：
    ///     1. 减少单元格预制体的复杂度，尽量使用简单的UI结构
    ///     2. 适当设置缓冲区大小，避免频繁创建和销毁单元格
    ///     3. 在SetCellData方法中避免执行昂贵的操作
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [AddComponentMenu("UI/Wrap Content")]
    public class UIWrapContent : ScrollRect
    {
        #region data structure
        /// <summary>
        /// 复杂视图适配器（支持多种单元格原型）
        /// </summary>
        public interface ICustomAdapter
        {
            /// <summary>
            /// 单原型模式
            /// </summary>
            bool SingleZygote { get; }

            /// <summary>
            /// 单元格类型
            /// <see cref="SingleZygote"/> 开启时，请返回固定值
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <returns>单元格类型ID</returns>
            int GetCellType(int index);

            /// <summary>
            /// 单元格原型
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>对应类型的原型RectTransform</returns>
            RectTransform GetCellZygote(int type);

            /// <summary>
            /// 单元格个数
            /// </summary>
            int GetCellCount();

            /// <summary>
            /// 设置单元格
            /// </summary>
            /// <param name="cell">单元格的RectTransform组件</param>
            /// <param name="index">单元格索引</param>
            void SetCellData(RectTransform cell, int index);
        }

        /// <summary>
        /// 简单视图适配器（获取Content首个子物体为单元格原型）
        /// </summary>
        public class ISimpleAdapter : ICustomAdapter
        {
            private readonly UIWrapContent wrapper;
            private readonly Func<int> onGetCellCount;
            private readonly Action<RectTransform, int> onSetCellData;

            /// <summary>
            /// 单原型模式
            /// </summary>
            public bool SingleZygote { get => true; }

            /// <summary>
            /// 单元格类型
            /// <see cref="SingleZygote"/> 开启时，请返回固定值
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <returns>单元格类型ID，始终为0</returns>
            public int GetCellType(int index) { return 0; }

            /// <summary>
            /// 单元格原型
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>Content的第一个子物体作为原型</returns>
            public RectTransform GetCellZygote(int type) { return wrapper.content.GetChild(0).GetComponent<RectTransform>(); }

            /// <summary>
            /// 单元格个数
            /// </summary>
            /// <returns>通过委托获取的单元格总数</returns>
            public int GetCellCount() { return onGetCellCount.Invoke(); }

            /// <summary>
            /// 设置单元格
            /// </summary>
            /// <param name="cell">单元格的RectTransform组件</param>
            /// <param name="index">单元格索引</param>
            public void SetCellData(RectTransform cell, int index) { onSetCellData.Invoke(cell, index); }

            /// <summary>
            /// 创建简单视图适配器实例。
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="onGetCellCount">获取单元格总数的委托</param>
            /// <param name="onSetCellData">设置单元格数据的委托</param>
            public ISimpleAdapter(UIWrapContent wrapper, Func<int> onGetCellCount, Action<RectTransform, int> onSetCellData)
            {
                this.wrapper = wrapper;
                this.onGetCellCount = onGetCellCount;
                this.onSetCellData = onSetCellData;
            }
        }

        /// <summary>
        /// 单元格扩展
        /// </summary>
        public readonly struct CellItem
        {
            /// <summary>
            /// 默认的单元格项，用作返回值的初始状态。
            /// </summary>
            public static readonly CellItem Default = new(0, 0, CellInfo.Default);

            /// <summary>
            /// 单元格在列表中的索引。
            /// </summary>
            public readonly int Index;

            /// <summary>
            /// 单元格的位置偏移。
            /// </summary>
            public readonly float Offset;

            /// <summary>
            /// 单元格信息。
            /// </summary>
            public readonly CellInfo Cell;

            /// <summary>
            /// 创建单元格项实例。
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <param name="offset">位置偏移</param>
            /// <param name="cell">单元格信息</param>
            public CellItem(int index, float offset, CellInfo cell)
            {
                Index = index;
                Offset = offset;
                Cell = cell;
            }

            /// <summary>
            /// 计算高度
            /// </summary>
            /// <returns>高度</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float CalcHeight() { return Cell.Transform.rect.height; }

            /// <summary>
            /// 计算宽度
            /// </summary>
            /// <returns>宽度</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float CalcWidth() { return Cell.Transform.rect.width; }
        }

        /// <summary>
        /// 单元格信息
        /// </summary>
        public readonly struct CellInfo
        {
            /// <summary>
            /// 默认的单元格信息，用作返回值的初始状态。
            /// </summary>
            public static readonly CellInfo Default = new(null, 0);

            /// <summary>
            /// 单元格的RectTransform组件。
            /// </summary>
            public readonly RectTransform Transform;

            /// <summary>
            /// 单元格类型ID。
            /// </summary>
            public readonly int Type;

            /// <summary>
            /// 创建单元格信息实例。
            /// </summary>
            /// <param name="transform">单元格的RectTransform组件</param>
            /// <param name="type">单元格类型ID</param>
            public CellInfo(RectTransform transform, int type)
            {
                Transform = transform;
                Type = type;
            }

            public override string ToString() { return $"{nameof(Transform)}: {Transform}, {nameof(Type)}: {Type}"; }
        }

        /// <summary>
        /// 单元格原型
        /// </summary>
        public readonly struct CellZygote
        {
            /// <summary>
            /// 单元格类型ID。
            /// </summary>
            public readonly int Type;

            /// <summary>
            /// 单元格宽度。
            /// </summary>
            public readonly float Width;

            /// <summary>
            /// 单元格高度。
            /// </summary>
            public readonly float Height;

            /// <summary>
            /// 锚点最小值。
            /// </summary>
            public readonly Vector2 AnchorMin;

            /// <summary>
            /// 锚点最大值。
            /// </summary>
            public readonly Vector2 AnchorMax;

            /// <summary>
            /// 中心点。
            /// </summary>
            public readonly Vector2 Pivot;

            /// <summary>
            /// 单元格原型的RectTransform组件。
            /// </summary>
            public readonly RectTransform Zygote;

            /// <summary>
            /// 创建单元格原型实例。
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <param name="width">单元格宽度</param>
            /// <param name="height">单元格高度</param>
            /// <param name="anchorMin">锚点最小值</param>
            /// <param name="anchorMax">锚点最大值</param>
            /// <param name="pivot">中心点</param>
            /// <param name="zygote">单元格原型的RectTransform组件</param>
            public CellZygote(int type, float width, float height, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, RectTransform zygote)
            {
                Type = type;
                Width = width;
                Height = height;
                AnchorMin = anchorMin;
                AnchorMax = anchorMax;
                Pivot = pivot;
                Zygote = zygote;
            }
        }

        /// <summary>
        /// 布局模式
        /// </summary>
        public enum LayoutMode
        {
            /// <summary>
            /// 垂直列表
            /// </summary>
            VerticalList,

            /// <summary>
            /// 水平列表
            /// </summary>
            HorizontalList,

            /// <summary>
            /// 垂直网格
            /// </summary>
            VerticalGrid,

            /// <summary>
            /// 水平网格
            /// </summary>
            HorizontalGrid
        }

        /// <summary>
        /// 布局基类
        /// </summary>
        public abstract class LayoutBase
        {
            /// <summary>
            /// 获取或设置循环模块。
            /// </summary>
            public virtual RecyleModule Recyle { get; set; }

            /// <summary>
            /// 是否为垂直滚动。
            /// </summary>
            public abstract bool Vertical { get; }

            /// <summary>
            /// 最小覆盖率 viewPort * MinCoverage
            /// 控制预加载区域的大小，值越大预加载的元素越多
            /// </summary>
            public virtual float MinCoverage => 1.64f;

            /// <summary>
            /// 计算内容在滑动方向的长度，此接口开销不小，请勿频繁请求
            /// 规定：这里的唯一为正方向距离（总是大于等于0）
            /// </summary>
            /// <returns>内容在滑动方向上的总长度</returns>
            public abstract float CalcContentLengthOfSlidingDir();

            /// <summary>
            /// 计算单元格位置偏移，此接口的开销不定，不建议频繁请求
            /// </summary>
            /// <param name="cell">单元格RectTransform</param>
            /// <param name="index">单元格索引</param>
            /// <returns>计算出的位置偏移</returns>
            public abstract Vector2 CalcCellOffset(RectTransform cell, int index);

            /// <summary>
            /// 获取单元格原型。
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>单元格原型数据</returns>
            public abstract CellZygote GetCellZygote(int type);

            /// <summary>
            /// 设置布局。
            /// 初始化布局并创建初始可见单元格。
            /// </summary>
            public abstract void Setup();

            /// <summary>
            /// 处理滚动值变化事件。
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="v">新的滚动值</param>
            public abstract void OnValueChanged(UIWrapContent wrapper, Vector2 v);

            /// <summary>
            /// 处理适配器变化事件。
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public abstract void OnAdapterChanged(UIWrapContent wrapper);

            /// <summary>
            /// 滚动到指定的归一化位置。
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            /// <param name="applyNormalized">是否应用归一化值到滚动条，默认为true</param>
            public abstract void ScrollTo(UIWrapContent wrapper, float normalized, bool applyNormalized = true);
        }

        /// <summary>
        /// 水平网格
        /// </summary>
        public class LayoutHorizontalGrid : LayoutBase
        {
            /// <summary>
            /// 创建水平网格布局实例
            /// </summary>
            /// <param name="row">网格的行数</param>
            /// <param name="horizontalSpace">水平间距</param>
            /// <param name="verticalSpace">垂直间距</param>
            public LayoutHorizontalGrid(int row, float horizontalSpace, float verticalSpace)
            {
                if (row < 1) row = 1;
                Row = row;
                HorizontalSpace = horizontalSpace;
                VerticalSpace = verticalSpace;
            }

            /// <summary>
            /// 网格的行数
            /// </summary>
            public int Row { get; }

            /// <summary>
            /// 水平间距
            /// </summary>
            public float HorizontalSpace { get; }

            /// <summary>
            /// 垂直间距
            /// </summary>
            public float VerticalSpace { get; }

            /// <summary>
            /// 最小覆盖率，用于控制预加载区域大小
            /// </summary>
            public override float MinCoverage => 1.5f;

            #region 抽象实现

            /// <summary>
            /// 是否为垂直滚动，水平网格返回false
            /// </summary>
            public override bool Vertical => false;

            /// <summary>
            /// 计算内容在水平方向的总长度
            /// </summary>
            /// <returns>内容总长度</returns>
            /// <exception cref="ArgumentException">当适配器不是单一原型模式时抛出异常</exception>
            public override float CalcContentLengthOfSlidingDir()
            {
                var ds = Recyle.Adapter;
                var count = ds.GetCellCount();
                if (count < 1) return 0f;
                if (!ds.SingleZygote) throw new ArgumentException("LayoutHorizontalGrid is unsupported in multiple zygote mode.");
                var zygote = Recyle.GetCellZygote(0);
                var column = (count - 1) / Row + 1;
                return zygote.Width * column + (column - 1) * HorizontalSpace; // [20230111]： 左右不留间隔
            }

            /// <summary>
            /// 计算指定索引单元格的位置偏移
            /// </summary>
            /// <param name="cell">单元格RectTransform</param>
            /// <param name="index">单元格索引</param>
            /// <returns>计算出的位置偏移</returns>
            /// <exception cref="ArgumentException">当适配器不是单一原型模式时抛出异常</exception>
            public override Vector2 CalcCellOffset(RectTransform cell, int index)
            {
                if (index < 1) return Vector2.zero;
                if (!Recyle.Adapter.SingleZygote) throw new ArgumentException("LayoutHorizontalGrid is unsupported in multiple zygote mode.");
                var zygote = Recyle.GetCellZygote(0);
                return CalcCellOffset(index, zygote.Width, zygote.Height);
            }

            /// <summary>
            /// 获取单元格原型数据
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>单元格原型数据</returns>
            public override CellZygote GetCellZygote(int type)
            {
                var zygote = Recyle.Adapter.GetCellZygote(type);
                var obj = zygote.gameObject;
                obj.SetActive(true);

                // 等比缩放
                var sizeDelta = zygote.sizeDelta;
                var cellHeight = (Recyle.Content.rect.height - VerticalSpace * (Row + 1)) / Row;
                var cellWidth = sizeDelta.x / sizeDelta.y * cellHeight;

                if (obj.scene.IsValid()) obj.SetActive(false);

                var value = new Vector2(0f, 1);
                return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
            }

            /// <summary>
            /// 初始化布局并创建初始可见单元格
            /// </summary>
            public override void Setup()
            {
                var rect = Recyle.Viewport.rect;
                edge = rect.width * (MinCoverage - 1) * 0.5f;
                var ds = Recyle.Adapter;
                if (ds.GetCellCount() < 1) return;

                var maxX = rect.width * MinCoverage;
                var count = ds.GetCellCount();
                var zygote = Recyle.GetCellZygote(0);
                var cellHeight = zygote.Height;
                var cellWidth = zygote.Width;

                var offset = 0f;
                var i = 0;
                while (i < count && (offset < maxX || i < MinCellCount))
                {
                    var cell = PopCachedPoolPoolAndPushToUsedPool(i, cellWidth, cellHeight);
                    ds.SetCellData(cell.Cell.Transform, i);
                    i++;
                    offset = cell.Offset;
                    if (i % Row == 0) offset += cellWidth;
                }
            }

            /// <summary>
            /// 处理滚动值变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="v">新的滚动值</param>
            public override void OnValueChanged(UIWrapContent wrapper, Vector2 v)
            {
                var normalized = wrapper.horizontalNormalizedPosition;
                var contentWidth = Recyle.Content.rect.width;
                var viewportWidth = Recyle.Viewport.rect.width;
                if (contentWidth <= viewportWidth) return;

                normalized = Mathf.Clamp(normalized, 0f, 1f);
                var posX = normalized * (contentWidth - viewportWidth);
                var dir = posX - prevOffset;
                if (Mathf.Abs(dir) < MinMovingDistance) return;

                ScrollTo(wrapper, normalized, false);
            }

            /// <summary>
            /// 处理适配器变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public override void OnAdapterChanged(UIWrapContent wrapper) { ScrollTo(wrapper, wrapper.horizontalNormalizedPosition); }

            /// <summary>
            /// 滚动到指定的归一化位置
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            /// <param name="applyNormalized">是否应用归一化值到滚动条，默认为true</param>
            public override void ScrollTo(UIWrapContent wrapper, float normalized, bool applyNormalized = true)
            {
                var adapter = Recyle.Adapter;
                var contentWidth = Recyle.Content.rect.width;
                var viewportWidth = Recyle.Viewport.rect.width;
                var posX = 0f;
                if (contentWidth > viewportWidth)
                {
                    normalized = Mathf.Clamp(normalized, 0f, 1f);
                    posX = normalized * (contentWidth - viewportWidth);
                }

                prevOffset = posX;
                var min = posX - edge;
                var max = posX + edge + viewportWidth;

                var zygote = Recyle.GetCellZygote(0);
                var cellHeight = zygote.Height;
                var cellWidth = zygote.Width;

                foreach (var item in Recyle.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = tf.anchoredPosition.x + tf.rect.width + HorizontalSpace;
                    if (min <= edge) break;
                    recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                for (var i = Recyle.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Recyle.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = tf.anchoredPosition.x;
                    if (max >= edge) break;
                    recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var existMinIndex = int.MaxValue;
                var existMaxIndex = int.MinValue;
                if (Recyle.UsedPool.Count > 0)
                {
                    existMinIndex = Recyle.LittleIndexFromUsedPool().Index;
                    existMaxIndex = Recyle.BigIndexFromUsedPool().Index;
                }

                var minCol = Mathf.Max((int)(min / (cellWidth + HorizontalSpace)), 0);
                var offset = cellWidth * minCol;
                var index = minCol * Row;

                var cellCount = adapter.GetCellCount();
                while (index < cellCount && max >= offset + cellWidth + HorizontalSpace)
                {
                    // 填充行
                    var col = index / Row;
                    while (col == index / Row && index < cellCount)
                    {
                        if (existMinIndex <= index && existMaxIndex >= index)
                        {
                            index++;
                            var curCol = index / Row;
                            offset = curCol * (cellWidth + HorizontalSpace);
                            continue;
                        }

                        var cur = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight);
                        adapter.SetCellData(cur.Cell.Transform, index);
                        offset = Mathf.Abs(cur.Offset);
                        index++;
                    }
                }

                if (applyNormalized) wrapper.horizontalNormalizedPosition = normalized;
            }

            #endregion

            #region 私有方法

            /// <summary>
            /// 从缓存池获取单元格并添加到已使用池
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <param name="cellWidth">单元格宽度</param>
            /// <param name="cellHeight">单元格高度</param>
            /// <returns>创建的单元格项</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private CellItem PopCachedPoolPoolAndPushToUsedPool(int index, float cellWidth, float cellHeight)
            {
                var cell = Recyle.PopFromCachedPool(index);
                var tf = cell.Transform;
                var ap = CalcCellOffset(index, cellWidth, cellHeight);
                tf.anchoredPosition = ap;
                var cur = new CellItem(index, ap.x, cell);
                Recyle.PushToUsedPool(cur);
                return cur;
            }

            /// <summary>
            /// 回收标记为待回收的单元格
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void RecyclingCellFromList()
            {
                if (recyclingPool.Count < 1) return;
                foreach (var cellExt in recyclingPool)
                {
                    Recyle.MoveToCachedPool(cellExt);
                }

                recyclingPool.Clear();
            }

            /// <summary>
            /// 计算单元格在网格中的位置偏移
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <param name="cellWidth">单元格宽度</param>
            /// <param name="cellHeight">单元格高度</param>
            /// <returns>计算出的位置偏移</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Vector2 CalcCellOffset(int index, float cellWidth, float cellHeight)
            {
                var col = index / Row;
                return new Vector2(
                    (cellWidth + HorizontalSpace) * col,
                    -(index % Row * (cellHeight + VerticalSpace) + VerticalSpace));
            }

            #endregion

            #region 私有属性

            /// <summary>
            /// 最小移动距离，小于此距离的滚动变化将被忽略
            /// </summary>
            private const float MinMovingDistance = 1f;

            /// <summary>
            /// 最小单元格数量，即使未达到视口覆盖区域，也至少创建这么多单元格
            /// </summary>
            private const int MinCellCount = 6;

            /// <summary>
            /// 待回收单元格列表
            /// </summary>
            private readonly List<CellItem> recyclingPool = new();

            /// <summary>
            /// 上一次的水平偏移位置
            /// </summary>
            private float prevOffset;

            /// <summary>
            /// 预加载区域的边缘值
            /// </summary>
            private float edge;

            #endregion
        }

        /// <summary>
        /// 垂直网格
        /// </summary>
        public class LayoutVerticalGrid : LayoutBase
        {
            /// <summary>
            /// 创建垂直网格布局实例
            /// </summary>
            /// <param name="column">网格的列数</param>
            /// <param name="horizontalSpace">水平间距</param>
            /// <param name="verticalSpace">垂直间距</param>
            public LayoutVerticalGrid(int column, float horizontalSpace, float verticalSpace)
            {
                if (column < 1) column = 1;
                Column = column;
                HorizontalSpace = horizontalSpace;
                VerticalSpace = verticalSpace;
            }

            /// <summary>
            /// 网格的列数
            /// </summary>
            public int Column { get; }

            /// <summary>
            /// 水平间距
            /// </summary>
            public float HorizontalSpace { get; }

            /// <summary>
            /// 垂直间距
            /// </summary>
            public float VerticalSpace { get; }

            /// <summary>
            /// 最小覆盖率，用于控制预加载区域大小
            /// </summary>
            public override float MinCoverage => 1.5f;

            #region 抽象实现

            /// <summary>
            /// 是否为垂直滚动，垂直网格返回true
            /// </summary>
            public override bool Vertical => true;

            /// <summary>
            /// 计算内容在垂直方向的总长度
            /// </summary>
            /// <returns>内容总长度</returns>
            /// <exception cref="ArgumentException">当适配器不是单一原型模式时抛出异常</exception>
            public override float CalcContentLengthOfSlidingDir()
            {
                var ds = Recyle.Adapter;
                var count = ds.GetCellCount();
                if (count < 1) return 0f;
                if (!ds.SingleZygote) throw new ArgumentException("LayoutVerticalGrid is unsupported in multiple zygote mode.");
                var zygote = Recyle.GetCellZygote(0);
                var rows = (count - 1) / Column + 1;
                return zygote.Height * rows + (rows - 1) * VerticalSpace; // [20230111]： 上下不留间隔
            }

            /// <summary>
            /// 计算指定索引单元格的位置偏移
            /// </summary>
            /// <param name="cell">单元格RectTransform</param>
            /// <param name="index">单元格索引</param>
            /// <returns>计算出的位置偏移</returns>
            /// <exception cref="ArgumentException">当适配器不是单一原型模式时抛出异常</exception>
            public override Vector2 CalcCellOffset(RectTransform cell, int index)
            {
                if (index < 1) return Vector2.zero;
                if (!Recyle.Adapter.SingleZygote) throw new ArgumentException("LayoutVerticalGrid is unsupported in multiple zygote mode.");
                var zygote = Recyle.GetCellZygote(0);
                return CalcCellOffset(index, zygote.Width, zygote.Height);
            }

            /// <summary>
            /// 获取单元格原型数据
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>单元格原型数据</returns>
            public override CellZygote GetCellZygote(int type)
            {
                var zygote = Recyle.Adapter.GetCellZygote(type);
                var obj = zygote.gameObject;
                obj.SetActive(true);

                // 等比缩放
                var sizeDelta = zygote.sizeDelta;
                var cellWidth = (Recyle.Content.rect.width - HorizontalSpace * (Column + 1)) / Column;
                var cellHeight = sizeDelta.y / sizeDelta.x * cellWidth;

                if (obj.scene.IsValid()) obj.SetActive(false);

                var value = new Vector2(0f, 1);
                return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
            }

            /// <summary>
            /// 初始化布局并创建初始可见单元格
            /// </summary>
            public override void Setup()
            {
                var rect = Recyle.Viewport.rect;
                edge = rect.height * (MinCoverage - 1) * 0.5f;
                var ds = Recyle.Adapter;
                if (ds.GetCellCount() < 1) return;

                var maxY = rect.height * MinCoverage;
                var count = ds.GetCellCount();
                var zygote = Recyle.GetCellZygote(0);
                var cellHeight = zygote.Height;
                var cellWidth = zygote.Width;

                var offset = 0f;
                var i = 0;
                while (i < count && (offset < maxY || i < MinCellCount))
                {
                    var cell = PopCachedPoolPoolAndPushToUsedPool(i, cellWidth, cellHeight);
                    ds.SetCellData(cell.Cell.Transform, i);
                    i++;
                    offset = Mathf.Abs(cell.Offset);
                    if (i % Column == 0) offset += cellHeight;
                }
            }

            /// <summary>
            /// 处理滚动值变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="v">新的滚动值</param>
            public override void OnValueChanged(UIWrapContent wrapper, Vector2 v)
            {
                var normalized = wrapper.verticalNormalizedPosition;
                var contentHeight = Recyle.Content.rect.height;
                var viewportHeight = Recyle.Viewport.rect.height;
                if (contentHeight <= viewportHeight) return;

                normalized = Mathf.Clamp(normalized, 0f, 1f);
                normalized = 1f - normalized;
                var posY = normalized * (contentHeight - viewportHeight);
                var dir = posY - prevOffset;
                if (Mathf.Abs(dir) < MinMovingDistance) return;

                ScrollTo(wrapper, normalized, false);
            }

            /// <summary>
            /// 处理适配器变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public override void OnAdapterChanged(UIWrapContent wrapper) { ScrollTo(wrapper, 1 - wrapper.verticalNormalizedPosition); }

            /// <summary>
            /// 滚动到指定的归一化位置
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            /// <param name="applyNormalized">是否应用归一化值到滚动条，默认为true</param>
            public override void ScrollTo(UIWrapContent wrapper, float normalized, bool applyNormalized = true)
            {
                var adapter = Recyle.Adapter;
                var contentHeight = Recyle.Content.rect.height;
                var viewportHeight = Recyle.Viewport.rect.height;
                var posY = 0f;
                if (contentHeight > viewportHeight)
                {
                    normalized = Mathf.Clamp(normalized, 0f, 1f);
                    posY = normalized * (contentHeight - viewportHeight);
                }

                prevOffset = posY;
                var min = posY - edge;
                var max = posY + edge + viewportHeight;

                var zygote = Recyle.GetCellZygote(0);
                var cellHeight = zygote.Height;
                var cellWidth = zygote.Width;

                foreach (var item in Recyle.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y) + tf.rect.height + VerticalSpace;
                    if (min <= edge) break;
                    recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                for (var i = Recyle.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Recyle.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y);
                    if (max >= edge) break;
                    recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var existMinIndex = int.MaxValue;
                var existMaxIndex = int.MinValue;
                if (Recyle.UsedPool.Count > 0)
                {
                    existMinIndex = Recyle.LittleIndexFromUsedPool().Index;
                    existMaxIndex = Recyle.BigIndexFromUsedPool().Index;
                }

                var minRow = Mathf.Max((int)(min / (cellHeight + VerticalSpace)), 0);
                var offset = (cellHeight + VerticalSpace) * minRow;
                var index = minRow * Column;

                var cellCount = adapter.GetCellCount();
                while (index < cellCount && max >= offset + cellHeight + VerticalSpace)
                {
                    // 填充列
                    var row = index / Column;
                    while (row == index / Column && index < cellCount)
                    {
                        if (existMinIndex <= index && existMaxIndex >= index)
                        {
                            index++;
                            var curRow = index / Column;
                            offset = curRow * (cellHeight + VerticalSpace);
                            continue;
                        }

                        var cur = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight);
                        adapter.SetCellData(cur.Cell.Transform, index);
                        offset = Mathf.Abs(cur.Offset);
                        index++;
                    }
                }

                if (applyNormalized) wrapper.verticalNormalizedPosition = 1f - normalized;
            }

            #endregion

            #region 私有方法

            /// <summary>
            /// 从缓存池获取单元格并添加到已使用池
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <param name="cellWidth">单元格宽度</param>
            /// <param name="cellHeight">单元格高度</param>
            /// <returns>创建的单元格项</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private CellItem PopCachedPoolPoolAndPushToUsedPool(int index, float cellWidth, float cellHeight)
            {
                var cell = Recyle.PopFromCachedPool(index);
                var tf = cell.Transform;
                var ap = CalcCellOffset(index, cellWidth, cellHeight);
                tf.anchoredPosition = ap;
                var cur = new CellItem(index, ap.y, cell);
                Recyle.PushToUsedPool(cur);
                return cur;
            }

            /// <summary>
            /// 回收标记为待回收的单元格
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void RecyclingCellFromList()
            {
                if (recyclingPool.Count < 1) return;
                foreach (var cellExt in recyclingPool)
                {
                    Recyle.MoveToCachedPool(cellExt);
                }

                recyclingPool.Clear();
            }

            /// <summary>
            /// 计算单元格在网格中的位置偏移
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <param name="cellWidth">单元格宽度</param>
            /// <param name="cellHeight">单元格高度</param>
            /// <returns>计算出的位置偏移</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Vector2 CalcCellOffset(int index, float cellWidth, float cellHeight)
            {
                var row = index / Column;
                return new Vector2(
                    index % Column * (cellWidth + HorizontalSpace) + HorizontalSpace,
                    -(cellHeight + VerticalSpace) * row);
            }

            #endregion

            #region 私有属性

            /// <summary>
            /// 最小移动距离，小于此距离的滚动变化将被忽略
            /// </summary>
            private const float MinMovingDistance = 1f;

            /// <summary>
            /// 最小单元格数量，即使未达到视口覆盖区域，也至少创建这么多单元格
            /// </summary>
            private const int MinCellCount = 6;

            /// <summary>
            /// 待回收单元格列表
            /// </summary>
            private readonly List<CellItem> recyclingPool = new();

            /// <summary>
            /// 上一次的垂直偏移位置
            /// </summary>
            private float prevOffset;

            /// <summary>
            /// 预加载区域的边缘值
            /// </summary>
            private float edge;

            #endregion
        }

        /// <summary>
        /// 水平列表
        /// </summary>
        public class LayoutHorizontalList : LayoutBase
        {
            /// <summary>
            /// 创建水平列表布局实例
            /// </summary>
            /// <param name="space">单元格间距</param>
            /// <param name="leftPadding">左侧内边距</param>
            /// <param name="rightPadding">右侧内边距</param>
            public LayoutHorizontalList(float space, float leftPadding, float rightPadding)
            {
                Space = space;
                LeftPadding = leftPadding;
                RightPadding = rightPadding;
            }

            /// <summary>
            /// 单元格间距
            /// </summary>
            public float Space { get; }

            /// <summary>
            /// 左侧内边距
            /// </summary>
            public float LeftPadding { get; }

            /// <summary>
            /// 右侧内边距
            /// </summary>
            public float RightPadding { get; }

            #region 抽象实现

            /// <summary>
            /// 是否为垂直滚动，水平列表返回false
            /// </summary>
            public override bool Vertical => false;

            /// <summary>
            /// 计算内容在水平方向的总长度
            /// </summary>
            /// <returns>内容总长度</returns>
            public override float CalcContentLengthOfSlidingDir()
            {
                var ds = Recyle.Adapter;
                var count = ds.GetCellCount();
                if (count < 1) return 0f;
                if (ds.SingleZygote)
                {
                    var zygote = Recyle.GetCellZygote(0);
                    return zygote.Width * count + Space * (count - 1) + LeftPadding + RightPadding;
                }
                var offset = 0f;
                for (var i = 0; i < count; i++)
                {
                    offset += Recyle.GetCellZygote(i).Width + (i > 0 ? Space : 0);
                }
                return offset + LeftPadding + RightPadding;
            }

            /// <summary>
            /// 计算指定索引单元格的位置偏移
            /// </summary>
            /// <param name="cell">单元格RectTransform</param>
            /// <param name="index">单元格索引</param>
            /// <returns>计算出的位置偏移</returns>
            public override Vector2 CalcCellOffset(RectTransform cell, int index)
            {
                if (index < 1) return Vector2.zero;
                var ds = Recyle.Adapter;
                if (ds.SingleZygote)
                {
                    var zygote = Recyle.GetCellZygote(0);
                    return new Vector2((zygote.Width + Space) * index, 0);
                }
                var offset = LeftPadding;
                for (var i = 0; i < index; i++)
                {
                    offset += Recyle.GetCellZygote(i).Width + Space;
                }
                return new Vector2(offset, 0);
            }

            /// <summary>
            /// 获取单元格原型数据
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>单元格原型数据</returns>
            public override CellZygote GetCellZygote(int type)
            {
                var zygote = Recyle.Adapter.GetCellZygote(type);
                var obj = zygote.gameObject;
                obj.SetActive(true);

                // 等比缩放
                var sizeDelta = zygote.sizeDelta;
                var cellHeight = Recyle.Content.rect.height;
                var cellWidth = sizeDelta.x / sizeDelta.y * cellHeight;

                if (obj.scene.IsValid()) obj.SetActive(false);

                var value = new Vector2(0, 0.5f);
                return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
            }

            /// <summary>
            /// 初始化布局并创建初始可见单元格
            /// </summary>
            public override void Setup()
            {
                var rect = Recyle.Viewport.rect;
                _edge = rect.width * (MinCoverage - 1) * 0.5f;
                var ds = Recyle.Adapter;
                if (ds.GetCellCount() < 1) return;

                var maxX = rect.width * MinCoverage;
                var offset = LeftPadding;
                var i = 0;

                var count = ds.GetCellCount();
                while (i < count && (offset < maxX || i < MinCellCount))
                {
                    var cell = Recyle.PopFromCachedPool(i);
                    var ap = cell.Transform.anchoredPosition;
                    ap.x = offset;
                    cell.Transform.anchoredPosition = ap;
                    var item = new CellItem(i, ap.x, cell);
                    Recyle.PushToUsedPool(item);
                    ds.SetCellData(cell.Transform, i);
                    offset += cell.Transform.rect.width + Space;
                    i++;
                }
            }

            /// <summary>
            /// 处理滚动值变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="v">新的滚动值</param>
            public override void OnValueChanged(UIWrapContent wrapper, Vector2 v)
            {
                var normalized = wrapper.horizontalNormalizedPosition;
                var contentWidth = Recyle.Content.rect.width;
                var viewportWidth = Recyle.Viewport.rect.width;
                if (contentWidth <= viewportWidth) return;

                normalized = Mathf.Clamp(normalized, 0f, 1f);
                var posX = normalized * (contentWidth - viewportWidth);
                var dir = posX - _prevOffset;
                if (Mathf.Abs(dir) < MinMovingDistance) return;

                ScrollTo(wrapper, normalized, false);
            }

            /// <summary>
            /// 处理适配器变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public override void OnAdapterChanged(UIWrapContent wrapper) { ScrollTo(wrapper, wrapper.horizontalNormalizedPosition); }

            /// <summary>
            /// 滚动到指定的归一化位置
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            /// <param name="applyNormalized">是否应用归一化值到滚动条，默认为true</param>
            public override void ScrollTo(UIWrapContent wrapper, float normalized, bool applyNormalized = true)
            {
                var contentWidth = Recyle.Content.rect.width;
                var viewportWidth = Recyle.Viewport.rect.width;

                var posX = 0f;
                if (contentWidth > viewportWidth)
                {
                    normalized = Mathf.Clamp(normalized, 0f, 1f);
                    posX = normalized * (contentWidth - viewportWidth);
                }

                _prevOffset = posX;
                var min = posX - _edge;
                var max = posX + _edge + viewportWidth;

                foreach (var item in Recyle.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y) + tf.rect.width + Space;
                    if (min <= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                for (var i = Recyle.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Recyle.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y);
                    if (max >= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var existMinIndex = int.MaxValue;
                var existMaxIndex = int.MinValue;
                if (Recyle.UsedPool.Count > 0)
                {
                    existMinIndex = Recyle.LittleIndexFromUsedPool().Index;
                    existMaxIndex = Recyle.BigIndexFromUsedPool().Index;
                }

                var count = Recyle.Adapter.GetCellCount();
                float offset = LeftPadding, width;
                for (var i = 0; i < count; i++, offset += width, offset += Space)
                {
                    width = Recyle.GetCellZygote(i).Width;
                    if (min > offset + width + Space) continue;
                    if (max < offset) break;
                    if (existMinIndex <= i && existMaxIndex >= i) continue;

                    var cell = Recyle.PopFromCachedPool(i);
                    var tf = cell.Transform;
                    var ap = tf.anchoredPosition;
                    ap.x = offset;
                    tf.anchoredPosition = ap;
                    var cur = new CellItem(i, ap.x, cell);
                    Recyle.PushToUsedPool(cur);
                    Recyle.Adapter.SetCellData(cell.Transform, i);
                }

                if (applyNormalized) wrapper.horizontalNormalizedPosition = normalized;
            }

            #endregion

            #region 私有方法

            /// <summary>
            /// 在指定方向绑定单元格
            /// </summary>
            /// <param name="item">起始单元格项</param>
            /// <param name="distance">绑定距离</param>
            /// <param name="littleIndex">是否向小索引方向绑定</param>
            private void BindCell(CellItem item, float distance, bool littleIndex)
            {
                var adapter = Recyle.Adapter;
                var o = 0f;
                var prev = item;

                while (o <= distance && (littleIndex ? prev.Index > 0 : prev.Index + 1 < adapter.GetCellCount()))
                {
                    float width;
                    int index;
                    Vector2 ap;
                    CellInfo cell;
                    RectTransform tf;

                    if (littleIndex)
                    {
                        index = prev.Index - 1;
                        cell = Recyle.PopFromCachedPool(index);
                        tf = cell.Transform;
                        width = tf.rect.width;
                        ap = tf.anchoredPosition;
                        ap.x = prev.Offset - width - Space;
                    }
                    else
                    {
                        index = prev.Index + 1;
                        cell = Recyle.PopFromCachedPool(index);
                        tf = cell.Transform;
                        width = prev.Cell.Transform.rect.width;
                        ap = tf.anchoredPosition;
                        ap.x = prev.Offset + width + Space;
                    }

                    tf.anchoredPosition = ap;
                    var cur = new CellItem(index, ap.x, cell);
                    Recyle.PushToUsedPool(cur);

                    adapter.SetCellData(cell.Transform, index);

                    prev = cur;
                    o += width + Space;
                }
            }

            /// <summary>
            /// 回收标记为待回收的单元格
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void RecyclingCellFromList()
            {
                if (_recyclingPool.Count < 1) return;
                foreach (var cellExt in _recyclingPool)
                {
                    Recyle.MoveToCachedPool(cellExt);
                }

                _recyclingPool.Clear();
            }

            #endregion

            #region 私有属性

            /// <summary>
            /// 最小移动距离，小于此距离的滚动变化将被忽略
            /// </summary>
            private const float MinMovingDistance = 1f;

            /// <summary>
            /// 最小单元格数量，即使未达到视口覆盖区域，也至少创建这么多单元格
            /// </summary>
            private const int MinCellCount = 3;

            /// <summary>
            /// 待回收单元格列表
            /// </summary>
            private readonly List<CellItem> _recyclingPool = new List<CellItem>();

            /// <summary>
            /// 上一次的水平偏移位置
            /// </summary>
            private float _prevOffset;

            /// <summary>
            /// 预加载区域的边缘值
            /// </summary>
            private float _edge;

            #endregion
        }

        /// <summary>
        /// 垂直列表
        /// </summary>
        public class LayoutVerticalList : LayoutBase
        {
            /// <summary>
            /// 创建垂直列表布局实例
            /// </summary>
            /// <param name="space">单元格间距</param>
            /// <param name="topPadding">顶部内边距</param>
            /// <param name="bottomPadding">底部内边距</param>
            public LayoutVerticalList(float space, float topPadding, float bottomPadding)
            {
                Space = space;
                TopPadding = topPadding;
                BottomPadding = bottomPadding;
            }

            /// <summary>
            /// 单元格间距
            /// </summary>
            public float Space { get; }

            /// <summary>
            /// 顶部内边距
            /// </summary>
            public float TopPadding { get; }

            /// <summary>
            /// 底部内边距
            /// </summary>
            public float BottomPadding { get; }

            #region 抽象实现

            /// <summary>
            /// 是否为垂直滚动，垂直列表返回true
            /// </summary>
            public override bool Vertical => true;

            /// <summary>
            /// 计算内容在垂直方向的总长度
            /// </summary>
            /// <returns>内容总长度</returns>
            public override float CalcContentLengthOfSlidingDir()
            {
                var ds = Recyle.Adapter;
                var count = ds.GetCellCount();
                if (count < 1) return 0f;
                if (ds.SingleZygote)
                {
                    var zygote = Recyle.GetCellZygote(0);
                    return zygote.Height * count + Space * (count - 1) + TopPadding + BottomPadding;
                }
                var offset = 0f;
                for (var i = 0; i < count; i++)
                {
                    offset += Recyle.GetCellZygote(i).Height + (i > 0 ? Space : 0);
                }
                return offset + TopPadding + BottomPadding;
            }

            /// <summary>
            /// 计算指定索引单元格的位置偏移
            /// </summary>
            /// <param name="cell">单元格RectTransform</param>
            /// <param name="index">单元格索引</param>
            /// <returns>计算出的位置偏移</returns>
            public override Vector2 CalcCellOffset(RectTransform cell, int index)
            {
                if (index < 1) return Vector2.zero;
                var ds = Recyle.Adapter;
                if (ds.SingleZygote)
                {
                    var zygote = Recyle.GetCellZygote(0);
                    return new Vector2(0, (zygote.Height + Space) * index);
                }
                var offset = TopPadding;
                for (var i = 0; i < index; i++)
                {
                    offset += Recyle.GetCellZygote(i).Height + Space;
                }
                return new Vector2(0, offset);
            }

            /// <summary>
            /// 获取单元格原型数据
            /// </summary>
            /// <param name="type">单元格类型ID</param>
            /// <returns>单元格原型数据</returns>
            public override CellZygote GetCellZygote(int type)
            {
                var zygote = Recyle.Adapter.GetCellZygote(type);
                var obj = zygote.gameObject;
                obj.SetActive(true);

                // 等比缩放
                var sizeDelta = zygote.sizeDelta;
                var cellWidth = Recyle.Content.rect.width;
                var cellHeight = sizeDelta.y / sizeDelta.x * cellWidth;

                if (obj.scene.IsValid()) obj.SetActive(false);

                var value = new Vector2(0.5f, 1);
                return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
            }

            /// <summary>
            /// 初始化布局并创建初始可见单元格
            /// </summary>
            public override void Setup()
            {
                var rect = Recyle.Viewport.rect;
                _edge = rect.height * (MinCoverage - 1) * 0.5f;
                var ds = Recyle.Adapter;
                if (ds.GetCellCount() < 1) return;

                var maxY = rect.height * MinCoverage;
                var offset = TopPadding;
                var i = 0;

                var count = ds.GetCellCount();
                while (i < count && (offset < maxY || i < MinCellCount))
                {
                    var cell = Recyle.PopFromCachedPool(i);
                    var ap = cell.Transform.anchoredPosition;
                    ap.y = -offset;
                    cell.Transform.anchoredPosition = ap;
                    var item = new CellItem(i, ap.y, cell);
                    Recyle.PushToUsedPool(item);
                    ds.SetCellData(cell.Transform, i);
                    offset += cell.Transform.rect.height + Space;
                    i++;
                }
            }

            /// <summary>
            /// 处理滚动值变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="v">新的滚动值</param>
            public override void OnValueChanged(UIWrapContent wrapper, Vector2 v)
            {
                var normalized = wrapper.verticalNormalizedPosition;
                var contentHeight = Recyle.Content.rect.height;
                var viewportHeight = Recyle.Viewport.rect.height;
                if (contentHeight <= viewportHeight) return;

                normalized = Mathf.Clamp(normalized, 0f, 1f);
                normalized = 1f - normalized;
                var posY = normalized * (contentHeight - viewportHeight);
                var dir = posY - _prevOffset;
                if (Mathf.Abs(dir) < MinMovingDistance) return;

                ScrollTo(wrapper, normalized, false);
            }

            /// <summary>
            /// 处理适配器变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public override void OnAdapterChanged(UIWrapContent wrapper) { ScrollTo(wrapper, 1 - wrapper.verticalNormalizedPosition); }

            /// <summary>
            /// 滚动到指定的归一化位置
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            /// <param name="applyNormalized">是否应用归一化值到滚动条，默认为true</param>
            public override void ScrollTo(UIWrapContent wrapper, float normalized, bool applyNormalized = true)
            {
                var contentHeight = Recyle.Content.rect.height;
                var viewportHeight = Recyle.Viewport.rect.height;

                var posY = 0f;
                if (contentHeight > viewportHeight)
                {
                    normalized = Mathf.Clamp(normalized, 0f, 1f);
                    posY = normalized * (contentHeight - viewportHeight);
                }

                _prevOffset = posY;
                var min = posY - _edge;
                var max = posY + _edge + viewportHeight;

                foreach (var item in Recyle.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y) + tf.rect.height + Space;
                    if (min <= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                for (var i = Recyle.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Recyle.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y);
                    if (max >= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var existMinIndex = int.MaxValue;
                var existMaxIndex = int.MinValue;
                if (Recyle.UsedPool.Count > 0)
                {
                    existMinIndex = Recyle.LittleIndexFromUsedPool().Index;
                    existMaxIndex = Recyle.BigIndexFromUsedPool().Index;
                }

                var count = Recyle.Adapter.GetCellCount();
                float offset = TopPadding, height;
                for (var i = 0; i < count; i++, offset += height, offset += Space)
                {
                    height = Recyle.GetCellZygote(i).Height;
                    if (min > offset + height + Space) continue;
                    if (max < offset) break;
                    if (existMinIndex <= i && existMaxIndex >= i) continue;

                    var cell = Recyle.PopFromCachedPool(i);
                    var tf = cell.Transform;
                    var ap = tf.anchoredPosition;
                    ap.y = -offset;
                    tf.anchoredPosition = ap;
                    var cur = new CellItem(i, ap.y, cell);
                    Recyle.PushToUsedPool(cur);
                    Recyle.Adapter.SetCellData(cell.Transform, i);
                }

                if (applyNormalized) wrapper.verticalNormalizedPosition = 1f - normalized;
            }

            #endregion

            #region 私有方法

            /// <summary>
            /// 在指定方向绑定单元格
            /// </summary>
            /// <param name="item">起始单元格项</param>
            /// <param name="distance">绑定距离</param>
            /// <param name="littleIndex">是否向小索引方向绑定</param>
            private void BindCell(CellItem item, float distance, bool littleIndex)
            {
                var adapter = Recyle.Adapter;
                var o = 0f;
                var prev = item;

                while (o <= distance && (littleIndex ? prev.Index > 0 : prev.Index + 1 < adapter.GetCellCount()))
                {
                    float height;
                    int index;
                    Vector2 ap;
                    CellInfo cell;
                    RectTransform tf;

                    if (littleIndex)
                    {
                        index = prev.Index - 1;
                        cell = Recyle.PopFromCachedPool(index);
                        tf = cell.Transform;
                        height = tf.rect.height;
                        ap = tf.anchoredPosition;
                        ap.y = prev.Offset + height + Space;
                    }
                    else
                    {
                        index = prev.Index + 1;
                        cell = Recyle.PopFromCachedPool(index);
                        tf = cell.Transform;
                        height = prev.Cell.Transform.rect.height;
                        ap = tf.anchoredPosition;
                        ap.y = prev.Offset - height - Space;
                    }

                    tf.anchoredPosition = ap;
                    var cur = new CellItem(index, ap.y, cell);
                    Recyle.PushToUsedPool(cur);

                    adapter.SetCellData(cell.Transform, index);

                    prev = cur;
                    o += height + Space;
                }
            }

            /// <summary>
            /// 回收标记为待回收的单元格
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void RecyclingCellFromList()
            {
                if (_recyclingPool.Count < 1) return;
                foreach (var cellExt in _recyclingPool)
                {
                    Recyle.MoveToCachedPool(cellExt);
                }

                _recyclingPool.Clear();
            }

            #endregion

            #region 私有属性

            /// <summary>
            /// 最小移动距离，小于此距离的滚动变化将被忽略
            /// </summary>
            private const float MinMovingDistance = 1f;

            /// <summary>
            /// 最小单元格数量，即使未达到视口覆盖区域，也至少创建这么多单元格
            /// </summary>
            private const int MinCellCount = 3;

            /// <summary>
            /// 待回收单元格列表
            /// </summary>
            private readonly List<CellItem> _recyclingPool = new List<CellItem>();

            /// <summary>
            /// 上一次的垂直偏移位置
            /// </summary>
            private float _prevOffset;

            /// <summary>
            /// 预加载区域的边缘值
            /// </summary>
            private float _edge;

            #endregion
        }

        /// <summary>
        /// 循环模块
        /// </summary>
        public class RecyleModule : IComparer<CellItem>
        {
            #region 公开成员、属性
            /// <summary>
            /// 获取或设置数据适配器
            /// </summary>
            public ICustomAdapter Adapter
            {
                get => adapter;
                protected set => adapter = value;
            }
            protected ICustomAdapter adapter;

            /// <summary>
            /// 获取或设置视口RectTransform
            /// </summary>
            public RectTransform Viewport
            {
                get => viewport;
                protected set => viewport = value;
            }
            protected RectTransform viewport;

            /// <summary>
            /// 获取或设置内容RectTransform
            /// </summary>
            public RectTransform Content
            {
                get => content;
                protected set => content = value;
            }
            protected RectTransform content;

            /// <summary>
            /// 获取或设置布局控制器
            /// </summary>
            public LayoutBase Layout
            {
                get => layout;
                protected set => layout = value;
            }
            protected LayoutBase layout;
            #endregion

            #region 公开方法
            /// <summary>
            /// 创建循环模块实例
            /// </summary>
            /// <param name="layout">布局控制器</param>
            /// <param name="adapter">数据适配器</param>
            /// <param name="viewport">视口RectTransform</param>
            /// <param name="content">内容RectTransform</param>
            public RecyleModule(LayoutBase layout, ICustomAdapter adapter, RectTransform viewport, RectTransform content)
            {
                layout.Recyle = this;
                Layout = layout;
                Adapter = adapter;
                Viewport = viewport;
                Content = content;
            }

            /// <summary>
            /// 初始化循环模块
            /// </summary>
            /// <param name="onInitialized">初始化完成回调</param>
            /// <returns>协程迭代器</returns>
            public IEnumerator Setup(Action onInitialized)
            {
                Inited = false;
                Cleanup();
                yield return null; // 等待一帧，保证获取到宽高

                if (Layout.Vertical)
                {
                    // Content.AnchorToTop();
                    var rect = Content.rect;
                    float width = rect.width, height = rect.height;

                    Content.anchorMin = new Vector2(0.5f, 1);
                    Content.anchorMax = new Vector2(0.5f, 1);
                    Content.pivot = new Vector2(0.5f, 1);

                    Content.sizeDelta = new Vector2(width, height);
                }
                else
                {
                    // Content.AnchorToLeft();
                    var rect = Content.rect;
                    float width = rect.width, height = rect.height;

                    Content.anchorMin = new Vector2(0, 0.5f);
                    Content.anchorMax = new Vector2(0, 0.5f);
                    Content.pivot = new Vector2(0, 0.5f);

                    Content.sizeDelta = new Vector2(width, height);
                }

                Content.anchoredPosition = Vector3.zero;

                SetContentSize();
                Layout.Setup();
                Inited = true;

                onInitialized?.Invoke();
            }

            /// <summary>
            /// 处理适配器变化事件
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            public void OnAdapterChanged(UIWrapContent wrapper)
            {
                if (!Inited || Layout == null) return;
                Cleanup();
                Adapter = wrapper.Adapter;
                SetContentSize();
                Layout.OnAdapterChanged(wrapper);
            }

            /// <summary>
            /// 滚动到指定位置
            /// </summary>
            /// <param name="wrapper">UIWrapContent实例</param>
            /// <param name="normalized">归一化位置值（0-1）</param>
            public void ScrollTo(UIWrapContent wrapper, float normalized)
            {
                if (!Inited || Layout == null) return;
                Layout.ScrollTo(wrapper, normalized);
            }

            /// <summary>
            /// 设置内容尺寸，根据布局计算内容在滑动方向上的长度
            /// </summary>
            protected void SetContentSize()
            {
                var contentSize = Layout.CalcContentLengthOfSlidingDir();
                var sizeDelta = Content.sizeDelta;
                sizeDelta = Layout.Vertical
                    ? new Vector2(sizeDelta.x, contentSize)
                    : new Vector2(contentSize, sizeDelta.y);
                Content.sizeDelta = sizeDelta;
            }

            /// <summary>
            /// 清理循环模块，销毁所有已创建的单元格
            /// </summary>
            protected void Cleanup()
            {
                foreach (var item in UsedPool) { Destroy(item.Cell.Transform.gameObject); }
                UsedPool.Clear();
                foreach (var ext in CachedPool) { Destroy(ext.Transform.gameObject); }
                CachedPool.Clear();
                Zygotes.Clear();
            }

            /// <summary>
            /// 获取单元格原型
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <returns>对应类型的单元格原型</returns>
            public CellZygote GetCellZygote(int index)
            {
                var type = Adapter.GetCellType(index);
                if (!Zygotes.TryGetValue(type, out var value))
                {
                    value = Layout.GetCellZygote(type);
                    Zygotes[type] = value;
                }
                return value;
            }

            /// <summary>
            /// 从缓存池获取单元格
            /// </summary>
            /// <param name="index">单元格索引</param>
            /// <returns>单元格信息</returns>
            public CellInfo PopFromCachedPool(int index)
            {
                var cell = CellInfo.Default;
                var type = Adapter.GetCellType(index);
                var count = CachedPool.Count;
                var i = 0;

                for (; i < count; i++)
                {
                    cell = CachedPool[i];
                    if (cell.Type != type) continue;
                    break;
                }

                if (count > 0 && i < count)
                {
                    CachedPool.RemoveAt(i);
                }
                else
                {
                    var zygote = GetCellZygote(index);
                    cell = CreateCell(zygote);
                }

                return cell;
            }

            /// <summary>
            /// 将单元格添加到已使用池
            /// </summary>
            /// <param name="item">单元格项</param>
            public void PushToUsedPool(CellItem item)
            {
                var index = UsedPool.BinarySearch(item, this);
                if (index < 0) UsedPool.Insert(~index, item);
            }

            /// <summary>
            /// 将单元格从已使用池移动到缓存池
            /// </summary>
            /// <param name="item">单元格项</param>
            public void MoveToCachedPool(CellItem item)
            {
                UsedPool.Remove(item);
                CachedPool.Add(item.Cell);
            }

            /// <summary>
            /// 比较两个单元格项的索引
            /// </summary>
            /// <param name="x">第一个单元格项</param>
            /// <param name="y">第二个单元格项</param>
            /// <returns>比较结果</returns>
            public int Compare(CellItem x, CellItem y) => x.Index.CompareTo(y.Index);

            /// <summary>
            /// 创建单元格实例
            /// </summary>
            /// <param name="zygote">单元格原型</param>
            /// <returns>新创建的单元格信息</returns>
            private CellInfo CreateCell(CellZygote zygote)
            {
                var cellObj = Instantiate(zygote.Zygote.gameObject, Content, false);
                var cell = cellObj.GetComponent<RectTransform>();
                cellObj.name = "Cell";
                cell.anchoredPosition = Vector2.zero;
                cell.anchorMin = zygote.AnchorMin;
                cell.anchorMax = zygote.AnchorMax;
                cell.pivot = zygote.Pivot;
                cell.sizeDelta = new Vector2(zygote.Width, zygote.Height);
                cellObj.SetActive(true);
                return new CellInfo(cell, zygote.Type);
            }
            #endregion

            #region 保护成员、属性
            /// <summary>
            /// 模块是否已初始化
            /// </summary>
            protected bool Inited;

            /// <summary>
            /// 已使用的单元格，此列表总是以索引从小到大排序
            /// </summary>
            public readonly List<CellItem> UsedPool = new List<CellItem>();

            /// <summary>
            /// 返回索引小端的单元格
            /// </summary>
            /// <returns>索引最小的单元格项</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CellItem LittleIndexFromUsedPool() => UsedPool.Count > 0 ? UsedPool[0] : CellItem.Default;

            /// <summary>
            /// 返回索引大端的单元格
            /// </summary>
            /// <returns>索引最大的单元格项</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CellItem BigIndexFromUsedPool() => UsedPool.Count > 0 ? UsedPool[UsedPool.Count - 1] : CellItem.Default;

            /// <summary>
            /// 缓存池（未使用的单元格）
            /// </summary>
            internal readonly List<CellInfo> CachedPool = new List<CellInfo>();

            /// <summary>
            /// 单元格原型（预制体）
            /// </summary>
            internal readonly Dictionary<int, CellZygote> Zygotes = new Dictionary<int, CellZygote>();
            #endregion
        }
        #endregion

        #region view logic
        /// <summary>
        /// 当前使用的布局模式。
        /// </summary>
        [SerializeField] private LayoutMode layout;

        /// <summary>
        /// 布局间隔或网格的行/列数。
        /// 在列表模式下表示间隔，在网格模式下表示行数或列数。
        /// </summary>
        [SerializeField] private int segment;

        /// <summary>
        /// 内边距。
        /// x为左/上内边距，y为右/下内边距。
        /// </summary>
        [SerializeField] private Vector2 padding;

        /// <summary>
        /// 滚动目标位置。
        /// 用于初始化完成前的滚动请求暂存。
        /// </summary>
        private float scrollTo = -1;

        /// <summary>
        /// 遍历每个显示项的委托。
        /// </summary>
        /// <param name="index">项的索引</param>
        /// <param name="item">项的RectTransform组件</param>
        public delegate void EachItemHandler(int index, RectTransform item);

        /// <summary>
        /// 获取或设置布局模式。
        /// </summary>
        public LayoutMode Layout
        {
            get => layout;
            set => layout = value;
        }

        /// <summary>
        /// 获取或设置循环模块。
        /// </summary>
        public RecyleModule Recyle { get; set; }

        /// <summary>
        /// 组件是否已完成初始化。
        /// </summary>
        public bool Inited { get; protected set; }

        /// <summary>
        /// 数据适配器实例。
        /// </summary>
        private ICustomAdapter adapter;

        /// <summary>
        /// 获取或设置数据适配器。
        /// 设置新适配器后会自动重新加载列表。
        /// </summary>
        public ICustomAdapter Adapter
        {
            get => adapter;
            set
            {
                adapter = value;
                if (Adapter != null) Reload();
            }
        }

        /// <summary>
        /// 组件启动时初始化
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            vertical = layout == LayoutMode.VerticalList || layout == LayoutMode.VerticalGrid;
            horizontal = !vertical;
            switch (layout)
            {
                case LayoutMode.HorizontalList:
                    Recyle = new RecyleModule(new LayoutHorizontalList(segment, padding.x, padding.y), Adapter, viewport, content);
                    break;
                case LayoutMode.VerticalList:
                    Recyle = new RecyleModule(new LayoutVerticalList(segment, padding.x, padding.y), Adapter, viewport, content);
                    break;
                case LayoutMode.VerticalGrid:
                    Recyle = new RecyleModule(new LayoutVerticalGrid(Math.Max(1, segment), padding.x, padding.y), Adapter, viewport, content);
                    break;
                case LayoutMode.HorizontalGrid:
                    Recyle = new RecyleModule(new LayoutHorizontalGrid(Math.Max(1, segment), padding.x, padding.y), Adapter, viewport, content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            onValueChanged.RemoveListener(OnValueChanged);
            StartCoroutine(Recyle.Setup(() =>
            {
                Inited = true;
                onValueChanged.AddListener(OnValueChanged);
                if (scrollTo >= 0)
                {
                    Recyle.ScrollTo(this, scrollTo);
                    scrollTo = -1;
                }
            }));
        }

        /// <summary>
        /// 处理滚动值变化事件。
        /// </summary>
        /// <param name="v">新的滚动值</param>
        private void OnValueChanged(Vector2 v) { if (Inited) Recyle.Layout.OnValueChanged(this, v); }

        /// <summary>
        /// 重载列表
        /// </summary>
        public void Reload()
        {
            if (!Inited) return;
            Recyle.OnAdapterChanged(this);
        }

        /// <summary>
        /// 跳转至指定位置
        /// </summary>
        /// <param name="normalized">[0 -> 从上到下/从左到右 -> 1]</param>
        public void ScrollTo(float normalized)
        {
            if (!Inited) scrollTo = normalized;
            else Recyle.ScrollTo(this, normalized);
        }

        /// <summary>
        /// 遍历显示的子节点
        /// </summary>
        /// <param name="handler">回调函数</param>
        public void EachItem(EachItemHandler handler)
        {
            if (!Inited) return;
            if (handler == null) return;
            for (int i = 0; i < Recyle.UsedPool.Count; i++)
            {
                var item = Recyle.UsedPool[i];
                handler.Invoke(item.Index, item.Cell.Transform);
            }
        }
        #endregion
    }
}
