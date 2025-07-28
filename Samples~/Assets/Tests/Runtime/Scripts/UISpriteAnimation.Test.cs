// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class TestUISpriteAnimation
{
    private GameObject obj;
    private UISpriteAnimation spriteAnimation;

    [SetUp]
    public void Setup()
    {
        // 创建测试对象
        obj = new GameObject("TestSpriteAnimation");
        spriteAnimation = obj.AddComponent<UISpriteAnimation>();
    }

    [TearDown]
    public void Cleanup()
    {
        // 清理测试对象
        Object.DestroyImmediate(obj);
    }

    [Test]
    public void Reset()
    {
        // 创建测试精灵 - 修复Rect参数
        Sprite sprite1 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        Sprite sprite2 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        Sprite[] testSprites = new Sprite[] { sprite1, sprite2 };
        spriteAnimation.Sprites = testSprites;

        // 调用Reset方法
        spriteAnimation.Reset();
        // 验证初始状态
        Assert.IsTrue(spriteAnimation.Active, "重置后应处于活动状态");
    }

    [UnityTest]
    public IEnumerator Active()
    {
        // 创建测试精灵 - 修复Rect参数
        Sprite sprite1 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        Sprite sprite2 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
        Sprite[] testSprites = new Sprite[] { sprite1, sprite2 };

        // 设置动画属性
        spriteAnimation.Sprites = testSprites;
        spriteAnimation.FrameRate = 10; // 每帧0.1秒
        spriteAnimation.Loop = true;

        // 重置动画
        spriteAnimation.Reset();

        // 等待足够时间让动画更新至少一帧
        yield return new WaitForSeconds(0.15f);
        Assert.IsTrue(spriteAnimation.Active, "动画应保持活动状态");

        spriteAnimation.Loop = false;
        yield return new WaitForSeconds(0.15f);
        Assert.IsFalse(spriteAnimation.Active, "动画应停止");
    }
}
#endif
