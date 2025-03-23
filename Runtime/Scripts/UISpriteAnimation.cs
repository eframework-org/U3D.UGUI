// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UI精灵动画组件。
    /// 用于实现图片序列帧动画效果，支持从图集或单独精灵加载动画帧。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 支持两种加载模式：图集模式和精灵模式
    /// - 支持按前缀筛选图集中的精灵
    /// - 可自定义动画帧率和循环播放设置
    /// - 支持在编辑器模式下预览动画效果
    /// - 自动适配原始尺寸，保证精灵动画比例正确
    /// 
    /// 使用手册
    /// 1. 组件设置
    /// 
    /// 1.1 添加组件
    ///     
    ///     选择UI游戏对象（必须有Image组件）
    ///     添加UISpriteAnimation组件
    ///     组件会自动获取Image组件用于显示动画
    /// 
    /// 1.2 精灵模式选择
    /// 
    ///     图集模式(Atlas)：从UIAtlas组件加载精灵
    ///     精灵模式(Sprite)：直接使用精灵数组
    /// 
    /// 2. 配置动画
    /// 
    /// 2.1 图集模式配置
    ///     
    ///     确保游戏对象上有UIAtlas组件
    ///     设置Prefix(前缀)筛选图集中的精灵
    ///     匹配前缀的精灵将按名称排序组成动画序列
    /// 
    /// 2.2 精灵模式配置
    ///     
    ///     直接将精灵数组赋值给Sprites属性
    ///     精灵将按数组顺序播放
    /// 
    /// 3. 动画控制
    /// 
    /// 3.1 基本设置
    ///     
    ///     Frame Rate：设置动画帧率（每秒帧数）
    ///     Loop：设置是否循环播放
    ///     Native Size：设置是否使用精灵原始大小
    /// 
    /// 3.2 运行时控制
    ///     
    ///     使用Reset()方法重置动画到第一帧并重新开始播放
    ///     通过Loop属性控制动画是否循环
    ///     通过FrameRate属性调整动画速度
    /// </code>
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
            /// 图集模式：从UIAtlas组件获取精灵序列。
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
        /// 引用当前游戏对象上的Image组件。
        /// 用于显示动画帧。
        /// </summary>
        private Image image;

        /// <summary>
        /// 引用当前游戏对象上的UIAtlas组件。
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
        /// 调用Rebuild方法准备动画序列。
        /// </summary>
        private void Start() { Rebuild(); }

        /// <summary>
        /// 每帧更新动画播放状态。
        /// 根据帧率和时间计算当前应显示的帧，并更新Image组件。
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
