// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TestUISpriteAnimationEditor
{
    [Test]
    public void CreateTemplate()
    {
        // 执行菜单操作创建Sprite Animation
        EditorApplication.ExecuteMenuItem("GameObject/UI/Sprite Animation");

        // 验证结果
        var createdObject = Selection.activeGameObject;
        Assert.IsNotNull(createdObject, "应当创建新的游戏对象");
        Assert.AreEqual("Sprite Animation", createdObject.name, "游戏对象名称应为'Sprite Animation'");

        // 验证组件
        var spriteAnimation = createdObject.GetComponent<UISpriteAnimation>();
        Assert.IsNotNull(spriteAnimation, "应当添加UISpriteAnimation组件");

        var image = createdObject.GetComponent<Image>();
        Assert.IsNotNull(image, "应当包含Image组件");

        // 清理
        if (createdObject != null) GameObject.DestroyImmediate(createdObject);

    }
}
#endif
