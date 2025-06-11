// Copyright (c) 2025 EFramework Organization. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using EFramework.Utility;
using EFramework.Editor;

namespace EFramework.UnityUI.Editor
{
    /// <summary>
    /// UIAtlas 编辑器工具，用于管理和处理 Unity 精灵图集。
    /// </summary>
    /// <remarks>
    /// <code>
    /// 功能特性
    /// - 在项目窗口中为 UIAtlas 预制体显示自定义图标
    /// - 提供通过 TexturePacker 创建和导入精灵图集的功能
    /// - 支持精灵图集的自动分割和重组
    /// - 管理纹理导入设置和精灵元数据
    /// - 支持透明度处理和边界修剪
    /// </code>
    /// 更多信息请参考模块文档。
    /// </remarks>
    public class UIAtlasEditor
    {
        internal static Texture2D icon;

        /// <summary>
        /// 编辑器初始化方法，加载图标并注册项目窗口绘制回调。
        /// </summary>
        [InitializeOnLoadMethod]
        internal static void OnInit()
        {
            var pkg = XEditor.Utility.FindPackage();
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/{pkg.name}/Editor/Resources/Icon/Atlas.png");
            if (icon) EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        /// <summary>
        /// 项目窗口绘制回调，为 UIAtlas 预制体添加自定义图标。
        /// </summary>
        /// <param name="guid">资源的 GUID</param>
        /// <param name="selectionRect">项目窗口中的绘制区域</param>
        internal static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith("prefab") && AssetDatabase.LoadAssetAtPath<UIAtlas>(path))
            {
                Rect iconRect = new Rect(selectionRect.x + selectionRect.width - 20, selectionRect.y, 16, 16);
                GUI.DrawTexture(iconRect, icon);
            }
        }

        /// <summary>
        /// 创建图集的菜单项处理方法。
        /// </summary>
        [MenuItem("Assets/Create/2D/Sheet Atlas")]
        internal static void Create()
        {
            if (Selection.assetGUIDs.Length == 0) XLog.Error("UIAtlasEditor.Create: non selection.");
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                if (string.IsNullOrEmpty(path)) XLog.Error("UIAtlasEditor.Create: selection path is empty.");
                else
                {
                    var rawPath = EditorUtility.SaveFolderPanel("Select Raw Path", XEnv.ProjectPath, null);
                    if (string.IsNullOrEmpty(rawPath)) XLog.Warn("UIAtlasEditor.Create: raw path is empty.");
                    else
                    {
                        rawPath = XFile.NormalizePath(Path.GetRelativePath(XEnv.ProjectPath, rawPath));
                        if (!XFile.HasDirectory(rawPath)) XLog.Error("UIAtlasEditor.Create: raw path doesn't exist: {0}", rawPath);
                        else
                        {
                            var atlasRoot = XFile.PathJoin(XFile.HasFile(path) ? Path.GetDirectoryName(path) : path, Path.GetFileName(rawPath));
                            if (!XFile.HasDirectory(atlasRoot)) XFile.CreateDirectory(atlasRoot);
                            var asset = Create(XFile.PathJoin(atlasRoot, Path.GetFileName(rawPath) + ".prefab"), rawPath);
                            if (asset) Selection.activeObject = asset;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建 UIAtlas 预制体资源。
        /// </summary>
        /// <param name="atlasPath">UIAtlas 预制体的保存路径</param>
        /// <param name="rawPath">包含精灵图片的素材路径</param>
        /// <returns>创建的 UIAtlas 资源对象</returns>
        public static Object Create(string atlasPath, string rawPath)
        {
            var go = new GameObject();
            go.AddComponent<UIAtlas>().RawPath = rawPath;

            var asset = PrefabUtility.SaveAsPrefabAsset(go, atlasPath);
            Object.DestroyImmediate(go);
            AssetDatabase.Refresh();

            return asset;
        }

        /// <summary>
        /// 资源后处理器，处理 UIAtlas 资源的导入和移动事件。
        /// </summary>
        internal class PostProcessor : AssetPostprocessor
        {
            /// <summary>
            /// 处理所有资源的后处理事件，导入 UIAtlas 资源。
            /// </summary>
            /// <param name="importedAssets">导入的资源路径</param>
            /// <param name="deletedAssets">删除的资源路径</param>
            /// <param name="movedAssets">移动的资源路径</param>
            /// <param name="movedFromAssetPaths">资源移动前的路径</param>
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (var path in importedAssets)
                {
                    if (path.EndsWith(".prefab") && AssetDatabase.LoadAssetAtPath<UIAtlas>(path))
                    {
                        Import(path);
                    }
                }

                foreach (var path in movedAssets)
                {
                    if (path.EndsWith(".prefab") && AssetDatabase.LoadAssetAtPath<UIAtlas>(path))
                    {
                        Import(path);
                    }
                }
            }
        }

        /// <summary>
        /// 导入 UIAtlas 资源，使用 TexturePacker 处理图集。
        /// </summary>
        /// <param name="path">UIAtlas 资源路径</param>
        /// <returns>导入是否成功</returns>
        public static bool Import(string path)
        {
            var atlas = AssetDatabase.LoadAssetAtPath<UIAtlas>(path);
            if (atlas == null)
            {
                XLog.Error("UIAtlasEditor.Import: null atlas at: {0}", path);
                return false;
            }

            if (!XFile.HasDirectory(atlas.RawPath))
            {
                XLog.Error("UIAtlasEditor.Import: raw path doesn't exist: {0}", atlas.RawPath);
                return false;
            }

            var atlasName = Path.GetFileNameWithoutExtension(path);
            var rawAtlasTex = XFile.PathJoin(Path.GetDirectoryName(atlas.RawPath), atlasName + ".png");
            var rawAtlasSheet = XFile.PathJoin(Path.GetDirectoryName(atlas.RawPath), atlasName + ".tpsheet");

            var argFile = XFile.PathJoin(Path.GetDirectoryName(atlas.RawPath), atlasName + ".arg");
            var arg = "--format unity-texture2d --force-publish --trim --disable-auto-alias --disable-rotation --force-squared --extrude 1";
            if (XFile.HasFile(argFile)) arg = XFile.OpenText(argFile);

            var success = false;

            try
            {
                var task = XEditor.Cmd.Run(
                                bin: XEditor.Cmd.Find("TexturePacker", "C:/Program Files/CodeAndWeb/TexturePacker/bin", "/Applications/TexturePacker.app/Contents/MacOS"),
                                args: new string[] { arg, $"--sheet \"{rawAtlasTex}\" --data \"{rawAtlasSheet}\" \"{atlas.RawPath}\"" });
                task.Wait();

                if (task.Result.Code == 0 && XFile.HasFile(rawAtlasTex) && XFile.HasFile(rawAtlasSheet))
                {
                    var sheetDesc = Parse(rawAtlasSheet);
                    if (sheetDesc == null) XLog.Error("UIAtlasEditor.Import: parse sheet info failed: {0}", rawAtlasSheet);
                    else
                    {
                        Draw(sheetDesc, atlas.RawPath, rawAtlasTex, arg.Contains("--trim") && !arg.Contains("--trim-mode None"));

                        var atlasDst = XFile.NormalizePath(Path.GetFullPath(Path.GetDirectoryName(path)));
                        var atlasTex = XFile.PathJoin(atlasDst, atlasName + ".png");
                        var atlasPrefab = XFile.PathJoin(atlasDst, atlasName + ".prefab");
                        if (!XFile.HasDirectory(atlasDst)) XFile.CreateDirectory(atlasDst);
                        File.Copy(rawAtlasTex, atlasTex, true);
                        AssetDatabase.Refresh();

                        atlasTex = atlasTex[(Application.dataPath.Length + 1)..];
                        atlasTex = "Assets/" + atlasTex;
                        atlasPrefab = atlasPrefab[(Application.dataPath.Length + 1)..];
                        atlasPrefab = "Assets/" + atlasPrefab;

                        var atlasPngImporter = AssetImporter.GetAtPath(atlasTex) as TextureImporter;
                        atlasPngImporter.textureType = TextureImporterType.Sprite;
                        if (atlasPngImporter.mipmapEnabled)
                        {
                            atlasPngImporter.mipmapEnabled = false;
                            AssetDatabase.ImportAsset(atlasTex);
                        }
                        AssetDatabase.Refresh();

                        if (Split(sheetDesc, atlasTex))
                        {
                            AssetDatabase.Refresh();
                            var sps = AssetDatabase.LoadAllAssetsAtPath(atlasTex).OfType<Sprite>().ToArray();
                            var go = AssetDatabase.LoadAssetAtPath<GameObject>(atlasPrefab);
                            atlas.Sprites = sps.OrderBy(ele => ele.name, System.StringComparer.OrdinalIgnoreCase).ToArray();
                            go = PrefabUtility.SavePrefabAsset(go);

                            if (icon) EditorGUIUtility.SetIconForObject(go, icon);
                        }
                        success = true;
                    }
                }
            }
            catch (System.Exception e) { XLog.Panic(e); }

            if (XFile.HasFile(rawAtlasTex)) XFile.DeleteFile(rawAtlasTex);
            if (XFile.HasFile(rawAtlasSheet)) XFile.DeleteFile(rawAtlasSheet);

            if (success) XLog.Debug("UIAtlasEditor.Import: import <a href=\"file:///{0}\">{1}</a> from <a href=\"file:///{2}\">{3}</a> succeeded.", Path.GetFullPath(path), path, Path.GetFullPath(atlas.RawPath), atlas.RawPath);
            else XLog.Error("UIAtlasEditor.Import: import <a href=\"file:///{0}\">{1}</a> from <a href=\"file:///{2}\">{3}</a> failed.", Path.GetFullPath(path), path, Path.GetFullPath(atlas.RawPath), atlas.RawPath);
            return success;
        }

        /// <summary>
        /// 图集描述类，存储图集的元数据信息。
        /// </summary>
        internal class SheetDesc
        {
            /// <summary>
            /// 精灵元数据数组
            /// </summary>
            public SpriteMetaData[] MetaData = null;

            /// <summary>
            /// 图集宽度
            /// </summary>
            public int Width = -1;

            /// <summary>
            /// 图集高度
            /// </summary>
            public int Height = -1;

            /// <summary>
            /// 是否启用轴心点
            /// </summary>
            public bool PivotPointsEnabled = true;

            /// <summary>
            /// 是否启用边界
            /// </summary>
            public bool BordersEnabled = false;

            /// <summary>
            /// 是否启用多边形
            /// </summary>
            public bool PolygonsEnabled = false;

            /// <summary>
            /// Alpha 是否为透明度
            /// </summary>
            public bool AlphaIsTransparency = true;
        }

        /// <summary>
        /// 解析 TexturePacker 生成的图集描述文件。
        /// </summary>
        /// <param name="sheetFile">图集描述文件路径</param>
        /// <returns>解析后的图集描述对象，解析失败返回 null</returns>
        internal static SheetDesc Parse(string sheetFile)
        {
            if (!XFile.HasFile(sheetFile)) return null;
            var lines = File.ReadAllLines(sheetFile);
            var nlines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!line.StartsWith("#")) nlines.Add(line); // elimate SmartUpdate
            }
            if (lines.Length != nlines.Count) File.WriteAllLines(sheetFile, nlines.ToArray());

            var sheetDesc = new SheetDesc();
            var spriteMetaDataDict = new SortedDictionary<string, SpriteMetaData>();

            foreach (string line in nlines)
            {
                if (line.StartsWith(":format="))
                {
                    int formatVersion = int.Parse(line[8..]);
                    if (formatVersion > 40300)
                    {
                        EditorUtility.DisplayDialog("Please update TexturePacker Importer", "Your TexturePacker Importer is too old to import '" + sheetFile + "', please load a new version from the asset store!\n\nEnsure that you have selected 'Unity - Texture2D sprite sheet' as data format in your TexturePacker project!", "Ok");
                        return null;
                    }
                }
                if (line.StartsWith(":size="))
                {
                    var sizeParts = line[6..].Split('x');
                    sheetDesc.Width = int.Parse(sizeParts[0]);
                    sheetDesc.Height = int.Parse(sizeParts[1]);
                }
                if (line.StartsWith(":pivotpoints="))
                {
                    string pivotPoints = line[13..].ToLower().Trim();
                    sheetDesc.PivotPointsEnabled = pivotPoints == "enabled";
                }
                if (line.StartsWith(":borders="))
                {
                    string borders = line[9..].ToLower().Trim();
                    sheetDesc.BordersEnabled = borders == "enabled";
                }
                if (line.StartsWith(":alphahandling=KeepTransparentPixels") || line.StartsWith(":alphahandling=PremultiplyAlpha"))
                {
                    sheetDesc.AlphaIsTransparency = false;
                }
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith(":"))
                {
                    continue;
                }
                var parts = line.Split(';');
                if (parts.Length < 7)
                {
                    EditorUtility.DisplayDialog("File format error", "Failed to import '" + sheetFile + "'", "Ok");
                    return null;
                }
                var smd = new SpriteMetaData
                {
                    name = unescapeName(parts[0]),
                    rect = new Rect(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture),
                        float.Parse(parts[4], CultureInfo.InvariantCulture)
                    )
                };
                var x = parts[5];
                var y = parts[6];
                sheetDesc.PivotPointsEnabled = sheetDesc.PivotPointsEnabled && parsePivot(x, y, ref smd);
                if (parts.Length >= 11)
                {
                    smd.border = new Vector4(
                        float.Parse(parts[7], CultureInfo.InvariantCulture),
                        float.Parse(parts[10], CultureInfo.InvariantCulture),
                        float.Parse(parts[8], CultureInfo.InvariantCulture),
                        float.Parse(parts[9], CultureInfo.InvariantCulture)
                    );
                    sheetDesc.PolygonsEnabled = true;
                }
                else
                {
                    smd.border = Vector4.zero;
                }
                spriteMetaDataDict.Add(smd.name, smd);
            }

            sheetDesc.MetaData = new SpriteMetaData[spriteMetaDataDict.Count];
            spriteMetaDataDict.Values.CopyTo(sheetDesc.MetaData, 0);

            if (!sheetDesc.PivotPointsEnabled)
            {
                for (int i = 0; i < sheetDesc.MetaData.Length; i++)
                {
                    sheetDesc.MetaData[i].pivot = new Vector2(0.5f, 0.5f);
                    sheetDesc.MetaData[i].alignment = 0;
                }
            }

            return sheetDesc;

            string unescapeName(string name) { return name.Replace("\\/", "/"); }

            bool parsePivot(string x, string y, ref SpriteMetaData smd)
            {
                if (float.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out float pivotX) &&
                    float.TryParse(y, NumberStyles.Float, CultureInfo.InvariantCulture, out float pivotY))
                {
                    smd.pivot = new Vector2(pivotX, pivotY);
                    smd.alignment = 0; // Assuming alignment is not specified, set to 0
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 绘制图集纹理，将各个精灵拼接到一个图集中。
        /// </summary>
        /// <param name="sheetDesc">图集描述对象</param>
        /// <param name="rawPath">精灵图片所在的素材路径</param>
        /// <param name="textureFile">目标图集纹理文件路径</param>
        /// <param name="isTrim">是否裁剪透明边缘</param>
        internal static void Draw(SheetDesc sheetDesc, string rawPath, string textureFile, bool isTrim = true)
        {
            static int getEdge(Bitmap bitmap, bool isStart, bool isHorizontal, bool isTrim = true)
            {
                if (!isTrim)
                {
                    if (isStart) return 0;
                    else return isHorizontal ? bitmap.Width - 1 : bitmap.Height - 1;
                }
                else
                {
                    int startEdge, endEdge, count;
                    if (isStart)
                    {
                        startEdge = 0;
                        endEdge = isHorizontal ? bitmap.Width - 1 : bitmap.Height - 1;
                        count = 1;
                    }
                    else
                    {
                        startEdge = isHorizontal ? bitmap.Width - 1 : bitmap.Height - 1;
                        endEdge = 0;
                        count = -1;
                    }
                    for (int i = startEdge; i != endEdge; i += count)
                    {
                        int limit = isHorizontal ? bitmap.Height : bitmap.Width;
                        for (int j = 0; j < limit; j++)
                        {
                            int x = isHorizontal ? i : j;
                            int y = isHorizontal ? j : i;
                            if (bitmap.GetPixel(x, y).ToArgb() != 0)
                            {
                                return i;
                            }
                        }
                    }
                }
                return 0;
            }

            var texWidth = sheetDesc.Width;
            var texHeight = sheetDesc.Height;

            using var texBitmap = new Bitmap(texWidth, texHeight);
            foreach (var smd in sheetDesc.MetaData)
            {
                var spritePath = XFile.PathJoin(rawPath, smd.name + ".png");
                if (!XFile.HasFile(spritePath)) spritePath = Path.ChangeExtension(spritePath, ".jpg");

                using var spriteBmp = new Bitmap(Image.FromFile(spritePath));
                var startX = getEdge(spriteBmp, true, true, isTrim);
                var endX = getEdge(spriteBmp, false, true, isTrim);
                var startY = getEdge(spriteBmp, true, false, isTrim);
                var endY = getEdge(spriteBmp, false, false, isTrim);

                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        texBitmap.SetPixel(
                            i - startX + (int)smd.rect.x,
                            j - startY + (int)(texHeight - smd.rect.y - smd.rect.height),
                            spriteBmp.GetPixel(i, j)
                        );
                    }
                }
            }

            texBitmap.Save(textureFile, ImageFormat.Png);
        }

        /// <summary>
        /// 拆分图集纹理为多个精灵。
        /// </summary>
        /// <param name="sheetDesc">图集描述对象</param>
        /// <param name="textureFile">图集纹理文件路径</param>
        /// <returns>是否成功拆分图集</returns>
        internal static bool Split(SheetDesc sheetDesc, string textureFile)
        {
            var importer = AssetImporter.GetAtPath(textureFile) as TextureImporter;
            if (importer == null)
            {
                XLog.Error("UIAtlasEditor.Split: not importer found for {0}", textureFile);
                return false;
            }

            copyMeta(importer.spritesheet, sheetDesc.MetaData, !sheetDesc.PivotPointsEnabled, !sheetDesc.BordersEnabled);
            if (!metaEqual(importer.spritesheet, sheetDesc.MetaData))
            {
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = sheetDesc.PolygonsEnabled ? SpriteMeshType.Tight : SpriteMeshType.FullRect;
                settings.textureType = TextureImporterType.Sprite;
                settings.alphaIsTransparency = sheetDesc.AlphaIsTransparency;
                settings.spriteMode = (int)SpriteImportMode.Multiple;
                importer.SetTextureSettings(settings);

                var dataProvider = getDataProvider(importer);
                var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();

                var oldIds = spriteNameFileIdDataProvider.GetNameFileIdPairs();
                var rects = getSpriteRects(sheetDesc);
                var ids = genSpriteIDs(oldIds, rects);

                dataProvider.SetSpriteRects(rects);
                spriteNameFileIdDataProvider.SetNameFileIdPairs(ids);
                dataProvider.Apply();
                importer.SaveAndReimport();

                return true;
            }

            return false;

            static bool metaEqual(SpriteMetaData[] meta1, SpriteMetaData[] meta2)
            {
                var flag = meta1.Length == meta2.Length;
                var num = 0;
                while (flag && num < meta1.Length)
                {
                    var m1 = meta1[num++];
                    var m2 = meta2.First(ele => ele.name == m1.name);
                    flag = m1.name == m2.name && m1.rect == m2.rect && m1.border == m2.border && m1.pivot == m2.pivot && m1.alignment == m2.alignment;
                }
                return flag;
            }

            static void copyMeta(SpriteMetaData[] oldMeta, SpriteMetaData[] newMeta, bool copyPivotPoints, bool copyBorders)
            {
                for (int i = 0; i < newMeta.Length; i++)
                {
                    for (int j = 0; j < oldMeta.Length; j++)
                    {
                        var val = oldMeta[j];
                        if (val.name == newMeta[i].name)
                        {
                            if (copyPivotPoints)
                            {
                                newMeta[i].pivot = val.pivot;
                                newMeta[i].alignment = val.alignment;
                            }
                            if (copyBorders)
                            {
                                newMeta[i].border = val.border;
                            }
                            break;
                        }
                    }
                }
            }

            static ISpriteEditorDataProvider getDataProvider(TextureImporter importer)
            {
                var dataProviderFactories = new SpriteDataProviderFactories();
                dataProviderFactories.Init();
                var dataProvider = dataProviderFactories.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();
                return dataProvider;
            }

            static SpriteRect[] getSpriteRects(SheetDesc desc)
            {
                var spriteCount = desc.MetaData.Length;
                SpriteRect[] rects = new SpriteRect[spriteCount];
                for (int i = 0; i < spriteCount; i++)
                {
                    SpriteRect sr = rects[i] = new SpriteRect();
                    SpriteMetaData smd = desc.MetaData[i];

                    sr.name = smd.name;
                    sr.rect = smd.rect;
                    sr.pivot = smd.pivot;
                    sr.border = smd.border;
                    sr.alignment = (SpriteAlignment)smd.alignment;
                }
                return rects;
            }

            static SpriteNameFileIdPair[] genSpriteIDs(IEnumerable<SpriteNameFileIdPair> oldIds,
                                                              SpriteRect[] sprites)
            {
                var newIds = new SpriteNameFileIdPair[sprites.Length];
                for (var i = 0; i < sprites.Length; i++)
                {
                    sprites[i].spriteID = idForName(oldIds, sprites[i].name);
                    newIds[i] = new SpriteNameFileIdPair(sprites[i].name, sprites[i].spriteID);
                }
                return newIds;
            }

            static GUID idForName(IEnumerable<SpriteNameFileIdPair> oldIds, string name)
            {
                foreach (var old in oldIds)
                {
                    if (old.name == name)
                    {
                        return old.GetFileGUID();
                    }
                }
                return GUID.Generate();
            }
        }
    }
}
