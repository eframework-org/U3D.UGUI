// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using EFramework.Utility;

namespace EFramework.UnityUI
{
    /// <summary>
    /// UI工具类，提供了一系列便捷的UI操作方法。
    /// 包含组件查找、属性设置、事件处理和资源加载等功能。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 提供UI组件快速查找和索引功能
    /// - 支持透明度、颜色等属性的便捷设置
    /// - 集成按钮点击事件和状态管理
    /// - 简化文本内容和图片设置操作
    /// - 支持网络图片异步加载和缓存管理
    /// - 提供UI布局刷新功能
    /// 
    /// 使用手册
    /// 1. 组件查找
    /// 
    /// 1.1 快速索引
    ///     
    ///     // 通过RectTransform查找子组件
    ///     Button btn = rectTransform.Index<Button>("ButtonName");
    ///     
    ///     // 通过Canvas查找子组件
    ///     Text txt = canvas.Index<Text>("TextName");
    /// 
    /// 2. 透明度控制
    /// 
    /// 2.1 设置组件透明度
    ///     
    ///     // 设置当前组件透明度为0.5
    ///     gameObject.SetGraphicAlpha(0.5f);
    ///     
    ///     // 设置子组件透明度为128（0-255范围）
    ///     gameObject.SetGraphicAlpha("ChildName", 128);
    /// 
    /// 3. 事件管理
    /// 
    /// 3.1 按钮点击事件
    ///     
    ///     // 为按钮设置点击事件
    ///     gameObject.SetButtonEvent((go) => {
    ///         Debug.Log("按钮被点击：" + go.name);
    ///     });
    ///     
    ///     // 启用/禁用按钮交互
    ///     gameObject.SetEventEnabled(false);
    /// 
    /// 4. 文本设置
    /// 
    /// 4.1 设置普通文本
    ///     
    ///     // 设置Text组件内容
    ///     gameObject.SetLabelText("新文本内容");
    ///     
    /// 4.2 设置TMP文本
    ///     
    ///     // 设置TextMeshProUGUI组件内容
    ///     gameObject.SetMeshProText("新文本内容");
    /// 
    /// 5. 图片管理
    /// 
    /// 5.1 设置精灵图片
    ///     
    ///     // 从图集中设置图片
    ///     gameObject.SetSpriteName("ChildImage", "SpriteName", atlasComponent);
    ///     
    /// 5.2 加载网络图片
    ///     
    ///     // 加载并显示网络图片
    ///     gameObject.SetRawImage("http://example.com/image.jpg");
    ///     
    ///     // 加载网络图片并禁用缓存
    ///     gameObject.SetRawImage("http://example.com/image.jpg", false);
    /// 
    /// 6. 布局刷新
    /// 
    /// 6.1 刷新UI布局
    ///     
    ///     // 重新计算布局
    ///     gameObject.RefreshObSort();
    /// </code>
    /// </remarks>
    public static class UIUtility
    {
        /// <summary>
        /// 快速索引组件。
        /// 通过路径查找目标节点并获取指定类型的组件。
        /// </summary>
        /// <param name="rect">当前的RectTransform组件</param>
        /// <param name="name">目标节点的名称或路径</param>
        /// <param name="type">要获取的组件类型，默认为null</param>
        /// <returns>找到的组件实例，如果未找到则返回null</returns>
        public static object Index(this RectTransform rect, string name, System.Type type = null) { if (rect) return XComp.Index(rect.gameObject, name, type); else return null; }

        /// <summary>
        /// 快速索引组件的泛型版本。
        /// 通过路径查找目标节点并获取指定类型的组件。
        /// </summary>
        /// <typeparam name="T">要获取的组件类型</typeparam>
        /// <param name="rect">当前的RectTransform组件</param>
        /// <param name="name">目标节点的名称或路径</param>
        /// <returns>找到的类型为T的组件实例，如果未找到则返回null</returns>
        public static T Index<T>(this RectTransform rect, string name) where T : class { if (rect) return XComp.Index<T>(rect.gameObject, name); else return null; }

        /// <summary>
        /// 快速索引组件。
        /// 通过路径在Canvas下查找目标节点并获取指定类型的组件。
        /// </summary>
        /// <param name="canvas">当前的Canvas组件</param>
        /// <param name="name">目标节点的名称或路径</param>
        /// <param name="type">要获取的组件类型，默认为null</param>
        /// <returns>找到的组件实例，如果未找到则返回null</returns>
        public static object Index(this Canvas canvas, string name, System.Type type = null) { if (canvas) return XComp.Index(canvas.gameObject, name, type); else return null; }

        /// <summary>
        /// 快速索引组件的泛型版本。
        /// 通过路径在Canvas下查找目标节点并获取指定类型的组件。
        /// </summary>
        /// <typeparam name="T">要获取的组件类型</typeparam>
        /// <param name="canvas">当前的Canvas组件</param>
        /// <param name="name">目标节点的名称或路径</param>
        /// <returns>找到的类型为T的组件实例，如果未找到则返回null</returns>
        public static T Index<T>(this Canvas canvas, string name) where T : class { if (canvas) return XComp.Index<T>(canvas.gameObject, name); else return null; }

        /// <summary>
        /// 设置图形组件的透明度。
        /// 使用0-255范围的整数值设置当前对象上图形组件的透明度。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="alpha">透明度值，范围为0-255，其中0表示完全透明，255表示完全不透明</param>
        /// <returns>修改后的Graphic组件</returns>
        public static Graphic SetGraphicAlpha(this Object rootObj, int alpha) { return SetGraphicAlpha(rootObj, null, alpha / 255f); }

        /// <summary>
        /// 设置图形组件的透明度。
        /// 使用0-1范围的浮点值设置当前对象上图形组件的透明度。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="alpha">透明度值，范围为0-1，其中0表示完全透明，1表示完全不透明</param>
        /// <returns>修改后的Graphic组件</returns>
        public static Graphic SetGraphicAlpha(this Object rootObj, float alpha) { return SetGraphicAlpha(rootObj, null, alpha); }

        /// <summary>
        /// 设置图形组件的透明度。
        /// 使用0-255范围的整数值设置指定路径对象上图形组件的透明度。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="alpha">透明度值，范围为0-255，其中0表示完全透明，255表示完全不透明</param>
        /// <returns>修改后的Graphic组件</returns>
        public static Graphic SetGraphicAlpha(this Object parentObj, string path, int alpha) { return SetGraphicAlpha(parentObj, path, alpha / 255f); }

        /// <summary>
        /// 设置图形组件的透明度。
        /// 使用0-1范围的浮点值设置指定路径对象上图形组件的透明度。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="alpha">透明度值，范围为0-1，其中0表示完全透明，1表示完全不透明</param>
        /// <returns>修改后的Graphic组件</returns>
        public static Graphic SetGraphicAlpha(this Object parentObj, string path, float alpha)
        {
            var graphic = parentObj.GetComponent(path, typeof(Graphic)) as Graphic;
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
            return graphic;
        }

        /// <summary>
        /// 设置按钮点击事件。
        /// 为当前对象上的按钮组件添加点击事件监听器。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="func">点击事件回调函数，参数为被点击的游戏对象</param>
        /// <returns>设置了事件的Button组件</returns>
        public static Button SetButtonEvent(this Object rootObj, System.Action<GameObject> func) { return SetButtonEvent(rootObj, null, func); }

        /// <summary>
        /// 设置按钮点击事件。
        /// 为指定路径对象上的按钮组件添加点击事件监听器。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="func">点击事件回调函数，参数为被点击的游戏对象</param>
        /// <returns>设置了事件的Button组件</returns>
        public static Button SetButtonEvent(this Object parentObj, string path, System.Action<GameObject> func)
        {
            var listener = parentObj.GetComponent(path, typeof(Button)) as Button;
            if (listener) listener.onClick.AddListener(() => { func?.Invoke(listener.gameObject); });
            return listener;
        }

        /// <summary>
        /// 设置事件是否启用。
        /// 控制当前对象上按钮组件的交互状态。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="enabled">是否启用交互，true表示启用，false表示禁用</param>
        public static void SetEventEnabled(this Object rootObj, bool enabled)
        {
            var btn = rootObj.GetComponent(typeof(Button)) as Button;
            if (btn) btn.interactable = enabled;
        }

        /// <summary>
        /// 设置事件是否启用。
        /// 控制指定路径对象上按钮组件的交互状态。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="enabled">是否启用交互，true表示启用，false表示禁用</param>
        public static void SetEventEnabled(this Object parentObj, string path, bool enabled)
        {
            var btn = parentObj.GetComponent(path, typeof(Button)) as Button;
            if (btn) btn.interactable = enabled;
        }

        /// <summary>
        /// 设置文本内容。
        /// 更新当前对象上Text组件的文本内容。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="content">要设置的文本内容，将自动转换为字符串</param>
        /// <returns>设置了内容的Text组件</returns>
        public static Text SetLabelText(this Object rootObj, object content)
        {
            if (content == null) return null;
            var label = rootObj.GetComponent(typeof(Text)) as Text;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            return null;
        }

        /// <summary>
        /// 设置文本内容。
        /// 更新指定路径对象上Text组件的文本内容。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="content">要设置的文本内容，将自动转换为字符串</param>
        /// <returns>设置了内容的Text组件</returns>
        public static Text SetLabelText(this Object parentObj, string path, object content)
        {
            if (content == null) return null;
            var label = parentObj.GetComponent(path, typeof(Text)) as Text;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            return null;
        }

        /// <summary>
        /// 设置TextMeshPro文本内容。
        /// 更新当前对象上TextMeshProUGUI组件的文本内容。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="content">要设置的文本内容，将自动转换为字符串</param>
        /// <returns>设置了内容的TextMeshProUGUI组件</returns>
        public static TextMeshProUGUI SetMeshProText(this Object rootObj, object content)
        {
            if (content == null) return null;
            var label = rootObj.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            return null;
        }

        /// <summary>
        /// 设置TextMeshPro文本内容。
        /// 更新指定路径对象上TextMeshProUGUI组件的文本内容。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="content">要设置的文本内容，将自动转换为字符串</param>
        /// <returns>设置了内容的TextMeshProUGUI组件</returns>
        public static TextMeshProUGUI SetMeshProText(this Object parentObj, string path, object content)
        {
            if (content == null) return null;
            var label = parentObj.GetComponent(path, typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            return null;
        }

        /// <summary>
        /// 设置图片精灵。
        /// 从指定图集中获取精灵并设置到指定路径对象的Image组件。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="name">要设置的精灵名称</param>
        /// <param name="atlas">精灵图集</param>
        /// <returns>设置了精灵的Image组件</returns>
        public static Image SetSpriteName(this Object parentObj, string path, string name, UIAtlas atlas)
        {
            var image = parentObj.GetComponent(path, typeof(Image)) as Image;
            if (image && atlas)
            {
                image.sprite = atlas.GetSprite(name);
                return image;
            }
            return null;
        }

        /// <summary>
        /// 设置图片精灵的透明度。
        /// 使用0-255范围的整数值设置当前对象上Image组件的透明度。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="alpha">透明度值，范围为0-255，其中0表示完全透明，255表示完全不透明</param>
        /// <returns>修改后的Image组件</returns>
        public static Image SetSpriteAlpha(this Object rootObj, int alpha) { return SetSpriteAlpha(rootObj, null, (float)alpha / 255); }

        /// <summary>
        /// 设置图片精灵的透明度。
        /// 使用0-255范围的整数值设置指定路径对象上Image组件的透明度。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="alpha">透明度值，范围为0-255，其中0表示完全透明，255表示完全不透明</param>
        /// <returns>修改后的Image组件</returns>
        public static Image SetSpriteAlpha(this Object parentObj, string path, int alpha) { return SetSpriteAlpha(parentObj, path, (float)alpha / 255); }

        /// <summary>
        /// 设置图片精灵的透明度。
        /// 使用0-1范围的浮点值设置当前对象上Image组件的透明度。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="alpha">透明度值，范围为0-1，其中0表示完全透明，1表示完全不透明</param>
        /// <returns>修改后的Image组件</returns>
        public static Image SetSpriteAlpha(this Object rootObj, float alpha) { return SetSpriteAlpha(rootObj, null, alpha); }

        /// <summary>
        /// 设置图片精灵的透明度。
        /// 使用0-1范围的浮点值设置指定路径对象上Image组件的透明度。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="alpha">透明度值，范围为0-1，其中0表示完全透明，1表示完全不透明</param>
        /// <returns>修改后的Image组件</returns>
        public static Image SetSpriteAlpha(this Object parentObj, string path, float alpha)
        {
            var image = parentObj.GetComponent(path, typeof(Image)) as Image;
            if (image)
            {
                var nc = new Color(image.color.r, image.color.g, image.color.b, alpha);
                image.color = nc;
            }
            return image;
        }

        /// <summary>
        /// 设置原始图片。
        /// 从网络URL加载图片并设置到当前对象上的RawImage组件，默认使用缓存。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="url">图片的网络URL</param>
        /// <returns>设置了图片的RawImage组件</returns>
        public static RawImage SetRawImage(this Object rootObj, string url) { return SetRawImage(rootObj, url, true); }

        /// <summary>
        /// 设置原始图片。
        /// 从网络URL加载图片并设置到当前对象上的RawImage组件，可控制是否使用缓存。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="url">图片的网络URL</param>
        /// <param name="incache">是否使用缓存，true表示使用，false表示不使用</param>
        /// <returns>设置了图片的RawImage组件</returns>
        public static RawImage SetRawImage(this Object rootObj, string url, bool incache)
        {
            var texture = rootObj.GetComponent(typeof(RawImage)) as RawImage;
            if (texture)
            {
                texture.texture = null;
                var done = false;
                if (incache)
                {
                    mCachedTextures.TryGetValue(url, out var tex);
                    if (tex)
                    {
                        texture.texture = tex;
                        done = true;
                    }
                }
                if (done == false) XLoom.StartCR(WWWTexture(url, texture, null));
            }
            return texture;
        }

        /// <summary>
        /// 设置原始图片。
        /// 从网络URL加载图片并设置到指定路径对象上的RawImage组件，默认使用缓存。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="url">图片的网络URL</param>
        /// <returns>设置了图片的RawImage组件</returns>
        public static RawImage SetRawImage(this Object parentObj, string path, string url) { return SetRawImage(parentObj, path, url, true); }

        /// <summary>
        /// 设置原始图片。
        /// 从网络URL加载图片并设置到指定路径对象上的RawImage组件，可控制是否使用缓存。
        /// </summary>
        /// <param name="parentObj">父对象</param>
        /// <param name="path">目标对象的路径，相对于父对象</param>
        /// <param name="url">图片的网络URL</param>
        /// <param name="incache">是否使用缓存，true表示使用，false表示不使用</param>
        /// <returns>设置了图片的RawImage组件</returns>
        public static RawImage SetRawImage(this Object parentObj, string path, string url, bool incache)
        {
            var texture = parentObj.GetComponent(path, typeof(RawImage)) as RawImage;
            if (texture)
            {
                texture.texture = null;
                var done = false;
                if (incache)
                {
                    mCachedTextures.TryGetValue(url, out var tex);
                    if (tex)
                    {
                        texture.texture = tex;
                        done = true;
                    }
                }
                if (done == false) XLoom.StartCR(WWWTexture(url, texture, null));
            }
            return texture;
        }

        /// <summary>
        /// 获取已缓存的贴图。
        /// 通过URL从缓存中获取之前加载过的贴图。
        /// </summary>
        /// <param name="url">贴图的网络URL</param>
        /// <returns>缓存的贴图，如果未缓存则返回null</returns>
        public static Texture2D GetTexture(string url)
        {
            Texture2D tex = null;
            if (string.IsNullOrEmpty(url) == false) mCachedTextures.TryGetValue(url, out tex);
            return tex;
        }

        /// <summary>
        /// 异步下载贴图。
        /// 从网络URL加载贴图，并在加载完成后执行回调，可控制是否使用缓存。
        /// </summary>
        /// <param name="url">贴图的网络URL</param>
        /// <param name="incache">是否使用缓存，true表示使用，false表示不使用</param>
        /// <param name="callback">加载完成后的回调函数</param>
        public static void WWWTexture(string url, bool incache, System.Action callback)
        {
            var done = false;
            if (incache)
            {
                mCachedTextures.TryGetValue(url, out var tex);
                if (tex)
                {
                    done = true;
                    callback?.Invoke();
                }
            }
            if (done == false) XLoom.StartCR(WWWTexture(url, null, callback));
        }

        /// <summary>
        /// 贴图缓存字典，用于存储已加载的网络贴图。
        /// 键为贴图的URL，值为加载的Texture2D对象。
        /// </summary>
        private static readonly Dictionary<string, Texture2D> mCachedTextures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// 异步加载贴图的协程方法。
        /// 通过UnityWebRequest下载贴图，并在下载完成后更新RawImage组件和缓存，并执行回调。
        /// </summary>
        /// <param name="url">贴图的网络URL</param>
        /// <param name="texture">要更新的RawImage组件，可为null</param>
        /// <param name="callback">加载完成后的回调函数</param>
        /// <returns>协程的迭代器</returns>
        private static IEnumerator WWWTexture(string url, RawImage texture, System.Action callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                callback?.Invoke();
                yield break;
            }
            using var req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();
            if (string.IsNullOrEmpty(req.error) == false)
            {
                XLog.Error("UIUtility.WWWTexture: error info: {0}, url is {1}.", req.error, url);
                callback?.Invoke();
                yield break;
            }
            if (req.isDone == false)
            {
                callback?.Invoke();
                yield break;
            }
            var tex = DownloadHandlerTexture.GetContent(req);
            if (tex == null)
            {
                callback?.Invoke();
                yield break;
            }
            if (mCachedTextures.ContainsKey(url)) mCachedTextures.Remove(url);
            mCachedTextures.Add(url, tex);
            if (texture) texture.texture = tex;
            callback?.Invoke();
        }

        /// <summary>
        /// 刷新UI布局。
        /// 强制更新指定对象上的布局控制器，重新计算和应用布局。
        /// </summary>
        /// <param name="rootObj">目标对象</param>
        /// <param name="path">目标对象的路径，相对于父对象，默认为空表示当前对象</param>
        public static void RefreshObSort(this Object rootObj, string path = "")
        {
            ILayoutController controller = rootObj.GetComponent(path, typeof(ContentSizeFitter)) as ContentSizeFitter;
            if (controller == null) controller = rootObj.GetComponent(path, typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
            if (controller != null)
            {
                controller.SetLayoutHorizontal();
                controller.SetLayoutVertical();
            }
        }
    }
}
