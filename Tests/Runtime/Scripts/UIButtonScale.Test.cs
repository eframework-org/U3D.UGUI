// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEngine;
using UnityEngine.UI;

public class TestUIButtonScale
{
    [Test]
    public void OnPointerDown()
    {
        // 创建测试对象
        var obj = new GameObject("TestButtonScale");
        obj.AddComponent<Button>();
        var buttonScale = obj.AddComponent<UIButtonScale>();
        buttonScale.OnPointerDown(null);
        Assert.AreEqual(new Vector3(0.95f, 0.95f, 0.95f), buttonScale.transform.localScale);
    }

    [Test]
    public void OnPointerUp()
    {
        var obj = new GameObject("TestButtonScale");
        obj.AddComponent<Button>();
        var buttonScale = obj.AddComponent<UIButtonScale>();
        buttonScale.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        buttonScale.OnPointerUp(null);
        Assert.AreEqual(Vector3.one, buttonScale.transform.localScale);
    }
}
#endif
