// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UISpriteAnimation 是一个 UI 精灵动画组件，用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 加载模式：支持图集和精灵图片两种加载模式
    /// - 参数控制：支持自定义动画帧率和循环播放等设置
    /// 
    /// 使用手册
    /// 1. 基本使用
    /// 
    ///     添加组件：
    ///       - 在 Hierarchey 面板中选择或创建一个游戏对象（必须有Image组件）
    ///       - 在 Inspector 窗口点击 Add Component
    ///       - 选择 UI/Sprite Animation 以添加组件
    /// 
    /// 2. 模式选择
    /// 
    ///     1. 图集模式(Atlas)：
    ///         - 从 UIAtlas 组件加载精灵
    ///         - 确保游戏对象上有 UIAtlas 组件
    ///         - 设置 Prefix(前缀) 筛选图集中的精灵
    ///         - 匹配前缀的精灵将按名称排序组成动画序列
    /// 
    ///     2. 精灵模式(Sprite)：
    ///         - 直接使用精灵数组
    ///         - 将精灵数组赋值给 Sprites 属性
    ///         - 精灵将按数组顺序播放
    /// 
    /// 3. 动画设置
    /// 
    ///     1. 基本设置：
    /// 
    ///         // 设置动画帧率（每秒帧数）
    ///         spriteAnimation.FrameRate = 30;
    ///         
    ///         // 设置是否循环播放
    ///         spriteAnimation.Loop = true;
    ///         
    ///         // 设置精灵数组（精灵模式下）
    ///         spriteAnimation.Sprites = mySprites;
    ///         
    ///         // 设置精灵名称前缀（图集模式下）
    ///         spriteAnimation.Prefix = "run_";
    /// 
    ///     2. 运行控制：
    /// 
    ///         // 重置动画到第一帧并重新开始播放
    ///         spriteAnimation.Reset();
    ///         
    ///         // 检查动画是否处于活动状态
    ///         if (spriteAnimation.Active) {
    ///             Debug.Log("动画正在播放");
    ///         }
    ///         
    ///         // 获取动画总帧数
    ///         int totalFrames = spriteAnimation.Frames;
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Sprite Animation")]
    public class UISpriteAnimation : MonoBehaviour
    {
        /// <summary>
        /// 精灵加载模式枚举。
        /// 定义动画帧的来源方式。
        /// </summary>
        public enum SpriteMode
        {
            /// <summary>
            /// 图集模式：从 UIAtlas 组件获取精灵序列。
            /// </summary>
            Atlas,

            /// <summary>
            /// 精灵模式：直接使用设置的精灵数组。
            /// </summary>
            Sprite
        }

        /// <summary>
        /// 当前使用的精灵模式。
        /// 决定动画帧的来源是图集还是精灵数组。
        /// </summary>
        [SerializeField] private SpriteMode spriteMode = SpriteMode.Atlas;

        /// <summary>
        /// 动画使用的精灵数组。
        /// 在精灵模式下，这些精灵将按顺序组成动画序列。
        /// </summary>
        [SerializeField] private Sprite[] sprites = new Sprite[0];

        /// <summary>
        /// 图集中精灵名称的前缀，用于过滤精灵。
        /// 在图集模式下，只有名称以此前缀开头的精灵才会被加入动画序列。
        /// </summary>
        [SerializeField] private string prefix = "";

        /// <summary>
        /// 动画帧率，每秒显示的帧数。
        /// 值越大动画播放速度越快。
        /// </summary>
        [SerializeField] private int frameRate = 30;

        /// <summary>
        /// 是否循环播放动画。
        /// 若为true，动画将在播放完毕后自动从头开始。
        /// </summary>
        [SerializeField] private bool loop = true;

        /// <summary>
        /// 是否使用精灵的原始大小。
        /// 若为true，将根据每一帧精灵的原始尺寸调整Image大小。
        /// </summary>
        [SerializeField] private bool nativeSize = false;

        /// <summary>
        /// 存储图集中符合前缀条件的精灵名称列表。
        /// 在图集模式下使用，按名称排序后按顺序播放。
        /// </summary>
        private readonly List<string> spriteNames = new();

        /// <summary>
        /// 引用当前游戏对象上的 Image 组件。
        /// 用于显示动画帧。
        /// </summary>
        private Image image;

        /// <summary>
        /// 引用当前游戏对象上的 UIAtlas 组件。
        /// 在图集模式下用于获取精灵。
        /// </summary>
        private UIAtlas atlas;

        /// <summary>
        /// 累积的时间增量，用于计算何时切换下一帧。
        /// 根据帧率和经过的时间决定何时显示下一帧。
        /// </summary>
        private float delta = 0f;

        /// <summary>
        /// 当前显示的帧索引。
        /// 指示当前正在显示的动画帧位置。
        /// </summary>
        private int index = 0;

        /// <summary>
        /// 动画是否处于活动状态。
        /// 若为false，动画将停止更新。
        /// </summary>
        private bool active = true;

        /// <summary>
        /// 获取或设置动画使用的精灵数组。
        /// 更改后会自动重建动画序列。
        /// </summary>
        public Sprite[] Sprites { get { return sprites; } set { if (sprites != value) { sprites = value; Rebuild(); } } }

        /// <summary>
        /// 获取动画的总帧数。
        /// 根据当前模式返回可用的帧数量。
        /// </summary>
        public int Frames { get { return Sprites.Length; } }

        /// <summary>
        /// 获取或设置动画的帧率。
        /// 控制动画播放的速度，每秒显示的帧数。
        /// </summary>
        public int FrameRate { get { return frameRate; } set { frameRate = value; } }

        /// <summary>
        /// 获取或设置图集中精灵名称的前缀。
        /// 更改后会自动重建动画序列。
        /// </summary>
        public string Prefix { get { return prefix; } set { if (prefix != value) { prefix = value; Rebuild(); } } }

        /// <summary>
        /// 获取或设置动画是否循环播放。
        /// 控制动画播放到最后一帧后的行为。
        /// </summary>
        public bool Loop { get { return loop; } set { loop = value; } }

        /// <summary>
        /// 获取动画是否处于活动状态。
        /// 判断动画是否正在播放中。
        /// </summary>
        public bool Active { get { return active; } }

        /// <summary>
        /// 组件启动时初始化。
        /// 调用 Rebuild 方法准备动画序列。
        /// </summary>
        private void Start() { Rebuild(); }

        /// <summary>
        /// 每帧更新动画播放状态。
        /// 根据帧率和时间计算当前应显示的帧，并更新 Image 组件。
        /// </summary>
        private void Update()
        {
            var count = spriteMode == SpriteMode.Atlas ? sprites.Length : spriteNames.Count;
            if (active && count > 1 && Application.isPlaying && frameRate > 0f)
            {
                delta += Time.deltaTime;
                float rate = 1f / frameRate;
                if (rate < delta)
                {
                    delta = (rate > 0f) ? delta - rate : 0f;
                    if (++index >= count)
                    {
                        index = 0;
                        active = Loop;
                    }
                    if (active)
                    {
                        if (spriteMode == SpriteMode.Atlas)
                        {
                            image.sprite = sprites[index];
                        }
                        else if (spriteMode == SpriteMode.Sprite)
                        {
                            if (atlas)
                            {
                                image.sprite = atlas.GetSprite(spriteNames[index]);
                            }
                        }
                        if (nativeSize) image.SetNativeSize();
                    }
                }
            }
        }

        /// <summary>
        /// 重建动画帧数据，根据当前模式准备精灵列表。
        /// 初始化组件引用并根据当前设置准备动画序列。
        /// </summary>
        private void Rebuild()
        {
            if (image == null) image = GetComponent<Image>();
            if (atlas == null) atlas = GetComponent<UIAtlas>();
            if (spriteMode == SpriteMode.Sprite)
            {
                spriteNames.Clear();
                if (image && atlas)
                {
                    var sprites = atlas.Sprites;
                    for (int i = 0, imax = sprites.Length; i < imax; ++i)
                    {
                        Sprite sprite = sprites[i];

                        if (string.IsNullOrEmpty(prefix) || sprite.name.StartsWith(prefix))
                        {
                            spriteNames.Add(sprite.name);
                        }
                    }
                    spriteNames.Sort();
                }
            }
        }

        /// <summary>
        /// 重置动画到初始状态，并显示第一帧。
        /// 可用于手动重新开始动画播放。
        /// </summary>
        public void Reset()
        {
            active = true;
            index = 0;
            if (image != null)
            {
                if (spriteMode == SpriteMode.Atlas)
                {
                    if (sprites.Length > 0)
                    {
                        image.sprite = sprites[index];
                    }
                }
                else if (spriteMode == SpriteMode.Sprite)
                {
                    if (atlas && spriteNames.Count > 0)
                    {
                        image.sprite = atlas.GetSprite(spriteNames[index]);
                    }
                }
                if (nativeSize) image.SetNativeSize();
            }
        }
    }
}
