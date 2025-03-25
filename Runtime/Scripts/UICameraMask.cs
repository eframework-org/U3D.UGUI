// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using UnityEngine;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// 相机遮罩组件。
    /// 提供屏幕淡入淡出的遮罩效果，可用于场景过渡。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 提供全屏幕遮罩淡入淡出效果
    /// - 支持自定义遮罩颜色和透明度
    /// - 单例模式设计，便于全局调用
    /// - 可控制过渡时间，适用于各种场景切换
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [AddComponentMenu("UI/Camera Mask")]
    public class UICameraMask : MonoBehaviour
    {
        /// <summary>
        /// 单例实例，方便全局访问。
        /// 可以通过 UICameraMask.Instance 在任何地方访问该组件。
        /// </summary>
        public static UICameraMask Instance;

        /// <summary>
        /// 屏幕遮罩的Canvas组，用于控制透明度。
        /// 需要在Inspector中手动赋值。
        /// </summary>
        public CanvasGroup ScreenMask;

        /// <summary>
        /// 遮罩图像组件。
        /// 用于设置遮罩的颜色，自动从ScreenMask获取。
        /// </summary>
        private Image image;

        /// <summary>
        /// 当前遮罩透明度值。
        /// 用于跟踪渐变过程中的透明度状态。
        /// </summary>
        private float currentAlpha;

        /// <summary>
        /// 遮罩透明度变化的持续时间。
        /// 控制淡入淡出效果的速度，默认为1秒。
        /// </summary>
        private float maskAlphaDuration = 1f;

        /// <summary>
        /// 最大透明度值。
        /// 控制遮罩的最大不透明度，范围为0-1。
        /// </summary>
        private float maxAlpha;

        /// <summary>
        /// 工作模式标记。
        /// 0表示正在处理渐变效果，-1表示渐变已完成。
        /// </summary>
        private float model;

        /// <summary>
        /// 是否为正向变化（淡出模式）。
        /// true表示从不透明到透明，false表示从透明到不透明。
        /// </summary>
        private bool isPositive;

        /// <summary>
        /// 初始化单例实例和图像组件。
        /// 在GameObject激活时自动调用。
        /// </summary>
        private void Awake()
        {
            Instance = this;
            image = ScreenMask.transform.GetComponent<Image>();
        }

        /// <summary>
        /// 每帧更新遮罩透明度变化。
        /// 仅在model为0（处理中）时执行渐变处理。
        /// </summary>
        private void Update() { if (model == 0) Process(); }

        /// <summary>
        /// 销毁时清理单例引用。
        /// 防止场景切换后出现空引用。
        /// </summary>
        private void OnDestroy() { Instance = null; }

        /// <summary>
        /// 处理遮罩透明度的渐变效果。
        /// 根据isPositive标志决定是淡入还是淡出效果。
        /// </summary>
        private void Process()
        {
            if (isPositive)
            {
                currentAlpha -= Time.deltaTime;
                if (ScreenMask != null)
                {
                    ScreenMask.alpha = currentAlpha / maskAlphaDuration;
                    if (currentAlpha <= 0)
                    {
                        ScreenMask.alpha = 0;
                        currentAlpha = 0;
                        model = -1;
                        isPositive = false;
                    }
                }
            }
            else
            {
                currentAlpha += Time.deltaTime;
                if (ScreenMask != null)
                {
                    ScreenMask.alpha = currentAlpha / maskAlphaDuration;
                    if (currentAlpha >= maxAlpha)
                    {
                        currentAlpha = maxAlpha;
                        isPositive = true;
                    }
                }
            }
        }

        /// <summary>
        /// 设置遮罩效果的目标参数。
        /// 启动一次完整的淡入淡出过程：先淡入（从透明到不透明），然后自动淡出（从不透明到透明）。
        /// </summary>
        /// <param name="color">遮罩的颜色，控制遮罩的颜色效果</param>
        /// <param name="time">渐变持续时间，控制淡入淡出的速度，默认为1秒</param>
        /// <param name="maxAlpha">最大透明度值，控制遮罩的最大不透明度，默认为1（完全不透明）</param>
        public void SetTarget(Color color, float time = 1f, float maxAlpha = 1)
        {
            isPositive = false;
            model = 0;
            if (time > 0) maskAlphaDuration = time;
            if (image) image.color = color;
            this.maxAlpha = maxAlpha;
        }
    }
}
