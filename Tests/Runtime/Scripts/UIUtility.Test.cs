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
using TMPro;
using EFramework.Utility;
using EFramework.Editor;

public class TestUIUtility
{
    private GameObject rootObj;
    private GameObject childObj;
    private Canvas canvas;
    private RectTransform rectTransform;

    [SetUp]
    public void Setup()
    {
        // 创建测试根对象
        rootObj = new GameObject("TestRoot");
        rectTransform = rootObj.AddComponent<RectTransform>();
        canvas = rootObj.AddComponent<Canvas>();

        // 创建子对象
        childObj = new GameObject("TestChild");
        childObj.transform.SetParent(rootObj.transform);
        childObj.AddComponent<RectTransform>();
    }

    [TearDown]
    public void Reset()
    {
        // 清理测试对象
        Object.DestroyImmediate(rootObj);
    }

    [Test]
    public void Index()
    {
        // 为子对象添加Image组件
        var childImage = childObj.AddComponent<Image>();

        // 使用RectTransform的Index方法查找子对象
        var result = rectTransform.Index("TestChild", typeof(Image));
        Assert.IsNotNull(result, "应能找到Image组件");
        Assert.AreEqual(childImage, result, "应返回正确的Image组件");

        // 使用泛型方法
        var typedResult1 = rectTransform.Index<Image>("TestChild");
        Assert.IsNotNull(typedResult1, "泛型方法应能找到Image组件");
        Assert.AreEqual(childImage, typedResult1, "泛型方法应返回正确的Image组件");

        // 为子对象添加Button组件
        var childButton = childObj.AddComponent<Button>();

        // 使用Canvas的Index方法查找子对象
        result = canvas.Index("TestChild", typeof(Button));
        Assert.IsNotNull(result, "应能找到Button组件");
        Assert.AreEqual(childButton, result, "应返回正确的Button组件");

        // 使用泛型方法
        var typedResult2 = canvas.Index<Button>("TestChild");
        Assert.IsNotNull(typedResult2, "泛型方法应能找到Button组件");
        Assert.AreEqual(childButton, typedResult2, "泛型方法应返回正确的Button组件");
    }

    [Test]
    public void SetGraphicAlpha()
    {
        // 添加Image组件
        var graphic = rootObj.AddComponent<Image>();
        graphic.color = Color.white; // 设置初始颜色

        // 测试直接设置透明度（整数）
        var result1 = UIUtility.SetGraphicAlpha(rootObj, 128);
        Assert.AreEqual(graphic, result1, "应返回正确的Graphic组件");
        Assert.AreEqual(0.5f, graphic.color.a, 0.01f, "透明度应设置为0.5");

        // 测试直接设置透明度（浮点数）
        var result2 = UIUtility.SetGraphicAlpha(rootObj, 0.25f);
        Assert.AreEqual(graphic, result2, "应返回正确的Graphic组件");
        Assert.AreEqual(0.25f, graphic.color.a, 0.01f, "透明度应设置为0.25");

        // 为子对象添加Text组件并测试路径方式
        var childText = childObj.AddComponent<Text>();
        childText.color = Color.white;

        // 测试路径设置透明度（整数）
        var result3 = rootObj.SetGraphicAlpha("TestChild", 64);
        Assert.AreEqual(childText, result3, "应返回正确的子对象Graphic组件");
        Assert.AreEqual(0.25f, childText.color.a, 0.01f, "子对象透明度应设置为0.25");

        // 测试路径设置透明度（浮点数）
        var result4 = rootObj.SetGraphicAlpha("TestChild", 0.75f);
        Assert.AreEqual(childText, result4, "应返回正确的子对象Graphic组件");
        Assert.AreEqual(0.75f, childText.color.a, 0.01f, "子对象透明度应设置为0.75");
    }

    [Test]
    public void SetButtonEvent()
    {
        // 添加Button组件
        var button = rootObj.AddComponent<Button>();
        bool callbackCalled = false;
        GameObject callbackObj = null;

        // 测试直接设置按钮事件
        var result1 = rootObj.SetButtonEvent((go) => { callbackCalled = true; callbackObj = go; });
        Assert.AreEqual(button, result1, "应返回正确的Button组件");

        // 模拟点击
        button.onClick.Invoke();
        Assert.IsTrue(callbackCalled, "回调应被调用");
        Assert.AreEqual(rootObj, callbackObj, "回调参数应为正确的GameObject");

        // 为子对象添加Button组件
        callbackCalled = false;
        callbackObj = null;
        var childButton = childObj.AddComponent<Button>();

        // 测试路径设置按钮事件
        var result2 = rootObj.SetButtonEvent("TestChild", (go) => { callbackCalled = true; callbackObj = go; });
        Assert.AreEqual(childButton, result2, "应返回正确的子对象Button组件");

        // 模拟点击
        childButton.onClick.Invoke();
        Assert.IsTrue(callbackCalled, "子对象回调应被调用");
        Assert.AreEqual(childObj, callbackObj, "子对象回调参数应为正确的GameObject");
    }

    [Test]
    public void SetEventEnabled()
    {
        // 添加Button组件
        var button = rootObj.AddComponent<Button>();
        button.interactable = false;

        // 测试直接设置事件启用状态
        rootObj.SetEventEnabled(true);
        Assert.IsTrue(button.interactable, "按钮应变为可交互");

        // 测试直接设置事件禁用状态
        rootObj.SetEventEnabled(false);
        Assert.IsFalse(button.interactable, "按钮应变为不可交互");

        // 为子对象添加Button组件
        var childButton = childObj.AddComponent<Button>();
        childButton.interactable = false;

        // 测试路径设置事件启用状态
        rootObj.SetEventEnabled("TestChild", true);
        Assert.IsTrue(childButton.interactable, "子对象按钮应变为可交互");

        // 测试路径设置事件禁用状态
        rootObj.SetEventEnabled("TestChild", false);
        Assert.IsFalse(childButton.interactable, "子对象按钮应变为不可交互");
    }

    [Test]
    public void SetLabelText()
    {
        // 添加Text组件
        var text = rootObj.AddComponent<Text>();
        text.text = "";

        // 测试直接设置文本
        var result1 = rootObj.SetLabelText("test content");
        Assert.AreEqual(text, result1, "应返回正确的Text组件");
        Assert.AreEqual("test content", text.text, "文本应正确设置");

        // 测试设置数字
        var result2 = rootObj.SetLabelText(123);
        Assert.AreEqual(text, result2, "应返回正确的Text组件");
        Assert.AreEqual("123", text.text, "数字应转换为文本并正确设置");

        // 为子对象添加Text组件
        var childText = childObj.AddComponent<Text>();
        childText.text = "";

        // 测试路径设置文本
        var result3 = rootObj.SetLabelText("TestChild", "child text content");
        Assert.AreEqual(childText, result3, "应返回正确的子对象Text组件");
        Assert.AreEqual("child text content", childText.text, "子对象文本应正确设置");
    }

    [Test]
    public void SetMeshProText()
    {
        // 添加TextMeshProUGUI组件
        var tmpText = rootObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = "";

        // 测试直接设置文本
        var result1 = rootObj.SetMeshProText("TMP test content");
        Assert.AreEqual(tmpText, result1, "应返回正确的TextMeshProUGUI组件");
        Assert.AreEqual("TMP test content", tmpText.text, "TMP文本应正确设置");

        // 为子对象添加TextMeshProUGUI组件
        var childTmpText = childObj.AddComponent<TextMeshProUGUI>();
        childTmpText.text = "";

        // 测试路径设置文本
        var result2 = rootObj.SetMeshProText("TestChild", "TMP child text content");
        Assert.AreEqual(childTmpText, result2, "应返回正确的子对象TextMeshProUGUI组件");
        Assert.AreEqual("TMP child text content", childTmpText.text, "子对象TMP文本应正确设置");
    }

    [Test]
    public void SetSpriteAlpha()
    {
        // 添加Image组件
        var image = rootObj.AddComponent<Image>();
        image.color = Color.white;

        // 测试直接设置透明度（整数）
        var result1 = rootObj.SetSpriteAlpha(128);
        Assert.AreEqual(image, result1, "应返回正确的Image组件");
        Assert.AreEqual(0.5f, image.color.a, 0.01f, "透明度应设置为0.5");

        // 测试直接设置透明度（浮点数）
        rootObj.SetSpriteAlpha(0.25f);
        Assert.AreEqual(0.25f, image.color.a, 0.01f, "透明度应设置为0.25");

        // 为子对象添加Image组件
        var childImage = childObj.AddComponent<Image>();
        childImage.color = Color.white;

        // 测试路径设置透明度（整数）
        rootObj.SetSpriteAlpha("TestChild", 64);
        Assert.AreEqual(0.25f, childImage.color.a, 0.01f, "子对象透明度应设置为0.25");

        // 测试路径设置透明度（浮点数）
        rootObj.SetSpriteAlpha("TestChild", 0.75f);
        Assert.AreEqual(0.75f, childImage.color.a, 0.01f, "子对象透明度应设置为0.75");
    }

    [Test]
    public void SetSpriteName()
    {
        // 创建一个临时的Sprite对象和UIAtlas对象
        Sprite sprite1 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        Sprite sprite2 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        sprite1.name = "TestSprite1";
        sprite2.name = "TestSprite2";
        var atlasObj = new GameObject("TestAtlas");
        var atlas = atlasObj.AddComponent<UIAtlas>();
        atlas.Sprites = new Sprite[] { sprite1, sprite2 };

        // 为子对象添加Image组件
        var childImage = childObj.AddComponent<Image>();

        // 测试设置精灵名称
        var result = rootObj.SetSpriteName("TestChild", "TestSprite1", atlas);
        Assert.AreEqual(childImage, result, "应返回正确的Image组件");
        Assert.AreEqual(sprite1, childImage.sprite, "精灵应正确设置");

        // 清理
        Object.DestroyImmediate(atlasObj);
    }

    [UnityTest]
    public IEnumerator SetRawImage()
    {
        // 添加RawImage组件
        var rawImage = rootObj.AddComponent<RawImage>();

        // 设置一个本地测试URI
        var testUri = "file://" + XFile.PathJoin(XEditor.Utility.FindPackage().resolvedPath, "Tests/Runtime/RawAssets/Square.png");

        // 测试直接设置图片
        var result1 = rootObj.SetRawImage(testUri, false);
        Assert.AreEqual(rawImage, result1, "应返回正确的RawImage组件");
        var childRawImage = childObj.AddComponent<RawImage>();
        // 测试路径设置图片
        var result2 = rootObj.SetRawImage("TestChild", testUri, false);
        Assert.AreEqual(childRawImage, result2, "应返回正确的子对象RawImage组件");

        // 等待一帧，让协程有机会运行
        yield return null;
        // 测试GetTexture方法
        var texture = UIUtility.GetTexture(testUri);
        Assert.IsNotNull(texture, "应返回正确的Texture2D对象");
    }
}
#endif
