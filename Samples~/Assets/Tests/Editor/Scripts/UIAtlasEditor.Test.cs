// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using EFramework.Utility;
using EFramework.Editor;
using EFramework.UnityUI.Editor;
using EFramework.UnityUI;

public class TestUIAtlasEditor
{
    const string TestDir = "Assets/Temp/TestUIAtlasEditor";
    readonly string TestAtlas = XFile.PathJoin(TestDir, "TestAtlas.prefab");
    readonly string TestRawPath = XFile.PathJoin(TestDir, "RawPath");
    string TestRawAssets;

    [OneTimeSetUp]
    public void Init()
    {
        TestRawAssets = XFile.PathJoin(XEnv.ProjectPath, "Assets/Tests/Runtime/RawAssets");
    }

    [SetUp]
    public void Setup()
    {
        if (!XFile.HasDirectory(TestRawPath)) XFile.CreateDirectory(TestRawPath);
        if (!XFile.HasDirectory(TestDir)) XFile.CreateDirectory(TestDir);
    }

    [TearDown]
    public void Reset()
    {
        if (XFile.HasDirectory(TestDir)) XFile.DeleteDirectory(TestDir);
    }

    [Test]
    public void OnLoad()
    {
        // Arrange
        UIAtlasEditor.icon = null;
        var originCount = EditorApplication.projectWindowItemOnGUI == null ? 0 : EditorApplication.projectWindowItemOnGUI.GetInvocationList()?.Length ?? 0;

        // Act
        (new UIAtlasEditor() as XEditor.Event.Internal.OnEditorLoad).Process();

        // Assert
        Assert.IsNotNull(UIAtlasEditor.icon, "icon 应当被加载到。");
        var addedCount = EditorApplication.projectWindowItemOnGUI.GetInvocationList().Length;
        Assert.AreEqual(originCount + 1, addedCount, "回调函数应当被注册。");
    }

    [Test]
    public void Create()
    {
        LogAssert.ignoreFailingMessages = true;  // 忽略所有错误日志
        var asset = UIAtlasEditor.Create(TestAtlas, TestRawPath);
        LogAssert.ignoreFailingMessages = false;    // 恢复正常行为

        // Assert
        Assert.IsNotNull(asset, "应该成功创建资产。");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TestAtlas);
        Assert.IsNotNull(prefab, "预制体应该存在。");

        var atlas = prefab.GetComponent<UIAtlas>();
        Assert.IsNotNull(atlas, "预制体应该包含 UIAtlas 组件。");
        Assert.AreEqual(TestRawPath, atlas.RawPath, "RawPath 应该被正确设置。");
    }

    [Test]
    public void Import()
    {
        var task = XEditor.Cmd.Run(
                               bin: XEditor.Cmd.Find("TexturePacker", "C:/Program Files/CodeAndWeb/TexturePacker/bin", "/Applications/TexturePacker.app/Contents/MacOS"),
                               args: new string[] { "--version" });
        task.Wait();
        Assert.IsTrue(task.Result.Code == 0, "TexturePacker 应当安装成功。");

        // 测试导入不存在的图集
        LogAssert.Expect(LogType.Error, new Regex(@"UIAtlasEditor\.Import: null atlas at: .*"));
        // 使用一个不存在的路径
        string nonExistentPath = "Assets/NonExistent/Atlas.prefab";
        bool result = UIAtlasEditor.Import(nonExistentPath);
        Assert.IsFalse(result, "当图集不存在时应返回 false。");

        // 测试导入不存在的RawPath
        LogAssert.Expect(LogType.Error, new Regex(@"UIAtlasEditor\.Import: raw path doesn't exist: .*"));

        // 创建图集预制体但设置不存在的RawPath
        var go = new GameObject("TestAtlas");
        var atlas = go.AddComponent<UIAtlas>();

        // 设置不存在的RawPath
        atlas.RawPath = "Assets/NonExistentRawPath";
        PrefabUtility.SaveAsPrefabAsset(go, TestAtlas);   // 保存预制体会触发Import

        // 测试导入成功
        atlas.RawPath = TestRawAssets;
        LogAssert.ignoreFailingMessages = true;
        PrefabUtility.SaveAsPrefabAsset(go, TestAtlas);   // 保存预制体会触发Import
        LogAssert.ignoreFailingMessages = false;

        // 验证预制体是否包含精灵引用
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TestAtlas);
        var prefaAtlas = prefab.GetComponent<UIAtlas>();
        Assert.IsNotNull(prefaAtlas, "预制体应该包含 UIAtlas 组件。");
        var atlasDir = Path.GetDirectoryName(TestAtlas);
        Assert.IsTrue(XFile.HasFile(XFile.PathJoin(atlasDir, "TestAtlas.png")), "TestAtlas.png 应当被生成。");
    }
}
#endif
