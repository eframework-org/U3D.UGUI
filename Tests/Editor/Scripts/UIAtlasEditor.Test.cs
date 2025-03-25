// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI.Editor;
using EFramework.UnityUI;
using UnityEditor;
using EFramework.Utility;
using UnityEngine;
using System.IO;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;
using EFramework.Editor;

public class TestUIAtlasEditor
{
    const string TestDir = "Assets/Temp/TestUIAtlasEditor";
    readonly string TestAtlas = XFile.PathJoin(TestDir, "TestAtlas.prefab");
    readonly string TestRawPath = XFile.PathJoin(TestDir, "RawPath");
    string TestRawAssets;

    [OneTimeSetUp]
    public void Init()
    {
        TestRawAssets = XFile.PathJoin(XEditor.Utility.FindPackage().resolvedPath, "Tests/Runtime/RawAssets");
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
    public void OnInit()
    {
        // Arrange
        UIAtlasEditor.icon = null;
        var originCount = EditorApplication.projectWindowItemOnGUI == null ? 0 : EditorApplication.projectWindowItemOnGUI.GetInvocationList()?.Length ?? 0;

        // Act
        UIAtlasEditor.OnInit();

        // Assert
        Assert.IsNotNull(UIAtlasEditor.icon, "icon应当被加载到");
        var addedCount = EditorApplication.projectWindowItemOnGUI.GetInvocationList().Length;
        Assert.AreEqual(originCount + 1, addedCount, "回调函数应当被注册");
    }

    [Test]
    public void Create()
    {
        LogAssert.ignoreFailingMessages = true;  // 忽略所有错误日志
        var asset = UIAtlasEditor.Create(TestAtlas, TestRawPath);
        LogAssert.ignoreFailingMessages = false;    // 恢复正常行为

        // Assert
        Assert.IsNotNull(asset, "应该成功创建资产");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TestAtlas);
        Assert.IsNotNull(prefab, "预制体应该存在");

        var atlas = prefab.GetComponent<UIAtlas>();
        Assert.IsNotNull(atlas, "预制体应该包含UIAtlas组件");
        Assert.AreEqual(TestRawPath, atlas.RawPath, "RawPath应该被正确设置");
    }

    [Test]
    public void Import()
    {
        // 测试导入不存在的图集
        LogAssert.Expect(LogType.Error, new Regex(@"UIAtlasEditor\.Import: null atlas at: .*"));
        // 使用一个不存在的路径
        string nonExistentPath = "Assets/NonExistent/Atlas.prefab";
        bool result = UIAtlasEditor.Import(nonExistentPath);
        Assert.IsFalse(result, "当图集不存在时应返回false");

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
        Assert.IsNotNull(prefaAtlas, "预制体应该包含UIAtlas组件");
        var atlasDir = Path.GetDirectoryName(TestAtlas);
        Assert.IsTrue(XFile.HasFile(XFile.PathJoin(atlasDir, "TestAtlas.png")), "TestAtlas.png应当被生成");
    }

    [Test]
    public void Parse()
    {
        // 测试不存在的文件
        var result = UIAtlasEditor.Parse("NonExistentFile.tpsheet");
        Assert.IsNull(result, "不存在的文件应返回null");

        // 测试正常文件
        result = UIAtlasEditor.Parse(XFile.PathJoin(TestRawAssets, "TestAtlas.tpsheet"));
        Assert.IsNotNull(result, "存在的文件应返回非null");
        Assert.AreEqual(result.MetaData.Length, 1, "应当包含1个精灵");
        Assert.AreEqual(result.MetaData[0].name, "Square", "应当包含Square");
        Assert.AreEqual(result.Width, 512, "宽度应为256");
        Assert.AreEqual(result.Height, 512, "高度应为256");
        Assert.IsFalse(result.BordersEnabled, "应当禁用Borders");
        Assert.IsTrue(result.PivotPointsEnabled, "应当启用PivotPoints");
        Assert.IsTrue(result.AlphaIsTransparency, "应当启用AlphaIsTransparency");
    }
}
#endif
