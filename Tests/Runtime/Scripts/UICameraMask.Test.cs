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

public class TestUICameraMask
{
    [UnityTest]
    public IEnumerator SetTarget()
    {
        // 创建测试对象
        var root = new GameObject("TestMask");
        root.AddComponent<Canvas>();
        var imageObj = new GameObject("Image");
        imageObj.SetActive(false);
        var canvasGroup = imageObj.AddComponent<CanvasGroup>();
        var image = imageObj.AddComponent<Image>();
        var mask = imageObj.AddComponent<UICameraMask>();
        mask.ScreenMask = canvasGroup;
        imageObj.SetActive(true);

        var testColor = Color.red;
        var duration = 2f;
        var maxAlpha = 0.5f;
        mask.SetTarget(testColor, duration, maxAlpha);
        Assert.AreEqual(testColor, image.color, "Image颜色应设置为红色");
        // 等待开始的一帧，确保Process被调用
        yield return null;
        // 验证开始变化 - 透明度应开始增加
        Assert.Greater(canvasGroup.alpha, 0f, "透明度应开始增加");
        Assert.Less(canvasGroup.alpha, maxAlpha, "透明度应小于最大值");
        yield return new WaitForSeconds(duration);
        Assert.AreEqual(0, canvasGroup.alpha, "透明度应该降为0");

        // 清理
        Object.DestroyImmediate(root);
    }
}
#endif
