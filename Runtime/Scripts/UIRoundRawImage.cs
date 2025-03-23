// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// 圆角原始图片组件。
    /// 继承自RawImage，通过自定义网格实现图片的圆角效果。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 为UI原始图片添加圆角效果
    /// - 自动计算和生成圆角网格
    /// - 支持自定义圆角半径和平滑度
    /// - 无需额外的Shader即可实现圆角效果
    /// 
    /// 使用手册
    /// 1. 组件添加
    /// 
    /// 1.1 添加到游戏对象
    ///     
    ///     在UI界面中选择或创建一个游戏对象
    ///     添加UIRoundRawImage组件（位于UI/Round Raw Image菜单下）
    ///     组件会自动替代默认的RawImage，提供圆角效果
    /// 
    /// 2. 属性设置
    /// 
    /// 2.1 基本属性
    ///     
    ///     Texture：设置显示的纹理
    ///     Color：设置图片的颜色
    ///     Material：设置自定义材质（可选）
    /// 
    /// 2.2 自定义圆角效果
    ///     
    ///     Radius：控制圆角大小（值越小圆角越大，默认为2）
    ///     TriangleNum：控制圆角平滑度（值越大越平滑，默认为10）
    /// 
    /// 3. 注意事项
    /// 
    /// 3.1 性能考量
    ///     
    ///     TriangleNum数值不宜设置过高，会增加渲染开销
    ///     对于不需要高度平滑的圆角，推荐保持默认值
    /// 
    /// 3.2 尺寸调整
    ///     
    ///     在调整RectTransform的大小时，圆角会自动适应
    ///     在极端宽高比下可能需要手动调整Radius以获得更好的效果
    /// </code>
    /// </remarks>
    [AddComponentMenu("UI/Round Raw Image")]
    public class UIRoundRawImage : RawImage
    {
        /// <summary>
        /// 内切圆半径参数。
        /// 控制圆角的大小，值越小圆角效果越明显，默认值为2（相当于图片十分之一的长度）。
        /// </summary>
        private float Radius = 2;

        /// <summary>
        /// 每个扇形三角形个数。
        /// 控制圆角的平滑程度，数值越大圆角越平滑，默认为10。
        /// </summary>
        private int TriangleNum = 10;

        /// <summary>
        /// 重写网格填充方法，生成带圆角效果的网格。
        /// 使用顶点和三角形构建圆角图片的几何结构。
        /// </summary>
        /// <param name="vh">顶点辅助器，用于添加和管理顶点数据</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            float tw = rectTransform.rect.width; // 图片的宽
            float th = rectTransform.rect.height; // 图片的高
            float twr = tw / 2;
            float thr = th / 2;

            if (Radius < 0)
                Radius = 0;
            float radius = tw / Radius; // 半径这里需要动态计算确保不会被拉伸
            if (radius > twr)
                radius = twr;
            if (radius < 0)
                radius = 0;
            if (TriangleNum <= 0)
                TriangleNum = 1;

            UIVertex vert = UIVertex.simpleVert;
            vert.color = color;
            //左边矩形
            AddVert(new Vector2(-twr, -thr + radius), tw, th, vh, vert);
            AddVert(new Vector2(-twr, thr - radius), tw, th, vh, vert);
            AddVert(new Vector2(-twr + radius, thr - radius), tw, th, vh, vert);
            AddVert(new Vector2(-twr + radius, -thr + radius), tw, th, vh, vert);
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
            //中间矩形
            AddVert(new Vector2(-twr + radius, -thr), tw, th, vh, vert);
            AddVert(new Vector2(-twr + radius, thr), tw, th, vh, vert);
            AddVert(new Vector2(twr - radius, thr), tw, th, vh, vert);
            AddVert(new Vector2(twr - radius, -thr), tw, th, vh, vert);
            vh.AddTriangle(4, 5, 6);
            vh.AddTriangle(4, 6, 7);
            //右边矩形
            AddVert(new Vector2(twr - radius, -thr + radius), tw, th, vh, vert);
            AddVert(new Vector2(twr - radius, thr - radius), tw, th, vh, vert);
            AddVert(new Vector2(twr, thr - radius), tw, th, vh, vert);
            AddVert(new Vector2(twr, -thr + radius), tw, th, vh, vert);
            vh.AddTriangle(8, 9, 10);
            vh.AddTriangle(8, 10, 11);

            List<Vector2> CirclePoint = new List<Vector2>();//圆心列表
            Vector2 pos0 = new Vector2(-twr + radius, -thr + radius);//左下角圆心
            Vector2 pos1 = new Vector2(-twr, -thr + radius);//决定首次旋转方向的点
            Vector2 pos2;
            CirclePoint.Add(pos0);
            CirclePoint.Add(pos1);
            pos0 = new Vector2(-twr + radius, thr - radius);//左上角圆心
            pos1 = new Vector2(-twr + radius, thr);
            CirclePoint.Add(pos0);
            CirclePoint.Add(pos1);
            pos0 = new Vector2(twr - radius, thr - radius);//右上角圆心
            pos1 = new Vector2(twr, thr - radius);
            CirclePoint.Add(pos0);
            CirclePoint.Add(pos1);
            pos0 = new Vector2(twr - radius, -thr + radius);//右下角圆心
            pos1 = new Vector2(twr - radius, -thr);
            CirclePoint.Add(pos0);
            CirclePoint.Add(pos1);
            float degreeDelta = (float)(Mathf.PI / 2 / TriangleNum);//每一份等腰三角形的角度 默认6份
            List<float> degreeDeltaList = new List<float>() { Mathf.PI, Mathf.PI / 2, 0, (float)3 / 2 * Mathf.PI };

            for (int j = 0; j < CirclePoint.Count; j += 2)
            {
                float curDegree = degreeDeltaList[j / 2];//当前的角度
                AddVert(CirclePoint[j], tw, th, vh, vert);//添加扇形区域所有三角形公共顶点
                int thrdIndex = vh.currentVertCount;//当前三角形第二顶点索引
                int TriangleVertIndex = vh.currentVertCount - 1;//一个扇形保持不变的顶点索引
                List<Vector2> pos2List = new List<Vector2>();
                for (int i = 0; i < TriangleNum; i++)
                {
                    curDegree += degreeDelta;
                    if (pos2List.Count == 0)
                    {
                        AddVert(CirclePoint[j + 1], tw, th, vh, vert);
                    }
                    else
                    {
                        vert.position = pos2List[i - 1];
                        vert.uv0 = new Vector2(pos2List[i - 1].x + 0.5f, pos2List[i - 1].y + 0.5f);
                    }
                    pos2 = new Vector2(CirclePoint[j].x + radius * Mathf.Cos(curDegree), CirclePoint[j].y + radius * Mathf.Sin(curDegree));
                    AddVert(pos2, tw, th, vh, vert);
                    vh.AddTriangle(TriangleVertIndex, thrdIndex, thrdIndex + 1);
                    thrdIndex++;
                    pos2List.Add(vert.position);
                }
            }
        }

        /// <summary>
        /// 计算顶点的UV坐标。
        /// 将网格顶点位置转换为纹理UV坐标，适应RawImage的纹理映射。
        /// </summary>
        /// <param name="vhs">顶点位置数组</param>
        /// <param name="tw">纹理宽度</param>
        /// <param name="th">纹理高度</param>
        /// <returns>对应的UV坐标数组</returns>
        protected Vector2[] GetTextureUVS(Vector2[] vhs, float tw, float th)
        {
            int count = vhs.Length;
            Vector2[] uvs = new Vector2[count];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vhs[i].x / tw + 0.5f, vhs[i].y / th + 0.5f);//矩形的uv坐标  因为uv坐标原点在左下角，vh坐标原点在中心 所以这里加0.5（uv取值范围0~1）
            }
            return uvs;
        }

        /// <summary>
        /// 添加顶点到网格。
        /// 设置顶点位置和UV坐标，并将其添加到顶点辅助器中。
        /// </summary>
        /// <param name="pos0">顶点位置</param>
        /// <param name="tw">纹理宽度</param>
        /// <param name="th">纹理高度</param>
        /// <param name="vh">顶点辅助器引用</param>
        /// <param name="vert">顶点数据</param>
        protected void AddVert(Vector2 pos0, float tw, float th, VertexHelper vh, UIVertex vert)
        {
            vert.position = pos0;
            vert.uv0 = GetTextureUVS(new[] { new Vector2(pos0.x, pos0.y) }, tw, th)[0];
            vh.AddVert(vert);
        }
    }
}
