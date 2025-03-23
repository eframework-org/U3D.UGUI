// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TestUIWrapContentEditor
{
    [Test]
    public void CreateTemplate()
    {
        // 执行菜单操作创建Wrap Content
        EditorApplication.ExecuteMenuItem("GameObject/UI/Wrap Content");

        // 验证结果
        var createdObject = Selection.activeGameObject;
        Assert.IsNotNull(createdObject, "应当创建新的游戏对象");
        Assert.AreEqual("Wrap Content", createdObject.name, "游戏对象名称应为'Wrap Content'");

        // 验证组件
        var wrapContent = createdObject.GetComponent<UIWrapContent>();
        Assert.IsNotNull(wrapContent, "应当添加UIWrapContent组件");

        // 验证层级结构（从ScrollRect生成，应该包含Viewport和Content）
        var viewport = createdObject.transform.Find("Viewport");
        Assert.IsNotNull(viewport, "应当包含Viewport子对象");
        var content = viewport?.Find("Content");
        Assert.IsNotNull(content, "应当包含Content子对象");

        // 验证ScrollRect的属性被转移到WrapContent
        Assert.IsNotNull(wrapContent.viewport, "viewport引用应正确设置");
        Assert.IsNotNull(wrapContent.content, "content引用应正确设置");

        // 清理
        if (createdObject != null) GameObject.DestroyImmediate(createdObject);
    }
}
#endif
