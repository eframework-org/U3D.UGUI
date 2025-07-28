// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using EFramework.UnityUI;
using UnityEngine;

public class TestUIAtlas
{
    [Test]
    public void GetSprite()
    {
        // 创建测试对象
        var gameObject = new GameObject("TestAtlas");
        var atlas = gameObject.AddComponent<UIAtlas>();

        // 准备测试精灵
        Sprite[] testSprites = new Sprite[2];
        testSprites[0] = Sprite.Create(Texture2D.whiteTexture, new Rect(), Vector2.zero);
        testSprites[0].name = "Sprite1";
        testSprites[1] = Sprite.Create(Texture2D.whiteTexture, new Rect(), Vector2.zero);
        testSprites[1].name = "Sprite2";

        // 设置精灵数组
        atlas.Sprites = testSprites;

        // 正常获取精灵图
        var sprite1 = atlas.GetSprite("Sprite1");
        Assert.IsNotNull(sprite1, "应当能找到存在的精灵");
        Assert.AreEqual("Sprite1", sprite1.name, "应当返回正确名称的精灵");

        // 获取不存在的精灵
        var nonExistSprite = atlas.GetSprite("NonExistentSprite");
        Assert.IsNull(nonExistSprite, "不存在的精灵应返回null");
        // 空名称
        var emptyNameSprite = atlas.GetSprite("");
        Assert.IsNull(emptyNameSprite, "空名称应返回null");

        // 清理
        Object.DestroyImmediate(gameObject);
    }
}
#endif
