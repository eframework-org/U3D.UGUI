// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI.Editor;
using UnityEditor;
using UnityEngine;
using EFramework.Utility;

public class TestUICanvasEditor
{
    const string TestDir = "Assets/Temp/TestCanvasEditor";

    [OneTimeSetUp]
    public void Init()
    {
        if (!XFile.HasDirectory(TestDir)) XFile.CreateDirectory(TestDir);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        if (XFile.HasDirectory(TestDir)) XFile.DeleteDirectory(TestDir);
    }

    [Test]
    public void OnInit()
    {
        // Arrange
        UICanvasEditor.icon = null;
        var originCount = EditorApplication.projectWindowItemOnGUI == null ? 0 : EditorApplication.projectWindowItemOnGUI.GetInvocationList()?.Length ?? 0;

        // Act
        UICanvasEditor.OnInit();

        // Assert
        Assert.IsNotNull(UICanvasEditor.icon, "icon应当被加载到");
        var addedCount = EditorApplication.projectWindowItemOnGUI.GetInvocationList().Length;
        Assert.AreEqual(originCount + 1, addedCount, "回调函数应当被注册");
    }

    [Test]
    public void OnPost()
    {
        // Arrange
        UICanvasEditor.OnInit();
        var canvasObj = new GameObject("TestCanvas");
        canvasObj.AddComponent(typeof(Canvas));

        try
        {
            // Assert
            var prefabPath = XFile.PathJoin(TestDir, "TestCanvas.prefab");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var actualIcon = EditorGUIUtility.GetIconForObject(prefab);
            Assert.AreEqual(UICanvasEditor.icon, actualIcon, "图标设置应当正确");
        }
        finally
        {
            // Cleanup
            GameObject.DestroyImmediate(canvasObj);
        }
    }
}
#endif
