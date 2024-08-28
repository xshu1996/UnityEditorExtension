using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;
using UnityEngine.U2D;

namespace Toolkit.Editor.ReferencesSpector
{
    /// <summary>
    /// 资源监控, 查看资源大小, 资源是否重复, 以及引用次数
    /// </summary>
    public class ResourceSpector : EditorWindow
    {
        #region Utils

        /// <summary>
        /// 获取资源相对于项目 Assets 目录下的相对路径 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAssetRelativePath(string path)
        {
            return "Assets" + path.Substring(Application.dataPath.Length).Replace("\\", "/");
        }

        /// <summary>
        /// 获取资源相绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAssetAbsolutePath(string path)
        {
            if (path.IndexOf(Application.dataPath, StringComparison.Ordinal) > -1) return path;
            
            return Application.dataPath.Substring(0, Application.dataPath.IndexOf(("Assets"))) + path;
        }

        /// <summary>
        /// 图片转base64
        /// </summary>
        /// <param name="absolutePath">图片绝对路径</param>
        /// <returns></returns>
        public static string ImageToBase64(string absolutePath)
        {
            FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            
            fs.Read(buffer, 0, (int)fs.Length);
            
            string base64String = Convert.ToBase64String(buffer);
            
            return base64String;
        }

        /// <summary>
        /// 获取Texture的占用硬盘大小
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int GetTextureMemorySize(Texture texture)
        {
            Type type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
            MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize",
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            
            if (methodInfo == null) return 0;
            
            return (int)methodInfo.Invoke(null, new object[] { texture });
        }

        /// <summary>
        /// 获取纹理内存占用大小
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static long GetTextureRuntimeMemorySize(Texture texture)
        {
            return Profiler.GetRuntimeMemorySizeLong(texture);
        }
        
        /// <summary>
        /// 判断资源路径是否属于插件包
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsInPlugins(string assetPath)
        {
            string lowerCasePath = assetPath.ToLower();
            return lowerCasePath.Contains("plugins");
        }

        /// <summary>
        /// 判断资源路径是否属于 Package
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsInPackages(string assetPath)
        {
            string lowerCasePath = assetPath.ToLower();
            return lowerCasePath.Contains("packages");
        }
        
        public static long GetSpriteAtlasDiskSize(string atlasPath)
        {
            // 加载图集
            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
        
            // 获取图集中所有纹理的路径
            Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
            spriteAtlas.GetSprites(sprites);
            
            foreach (var sprite in sprites)
            {
                Debug.Log(sprite.name);
            }
            
            Debug.Log(EditorUtility.FormatBytes(new FileInfo(atlasPath).Length));
            

            string[] texturePaths = sprites
                .Select(sprite => Path.Combine(Path.GetDirectoryName(atlasPath) ?? string.Empty, sprite.name)).ToArray();
            
            // 计算所有纹理的总大小
            long totalSize = 0;
            
            foreach (string texturePath in texturePaths)
            {
                FileInfo fileInfo = new FileInfo(texturePath + ".png");
                if (fileInfo.Exists)
                {
                    totalSize += fileInfo.Length;
                }
            }
            
            Debug.Log(EditorUtility.FormatBytes(totalSize));
            
            return totalSize;
        }
        
        #endregion

        
        [MenuItem("Assets/References Spector")]
        static void OpenWindow()
        {
            ResourceSpector window = GetWindow<ResourceSpector>();
            window.minSize = new Vector2(800, 500);
            window.titleContent = new GUIContent() { text = nameof(ResourceSpector) };
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                ignorePackages = EditorGUILayout.Toggle("IgnorePackages", ignorePackages, GUILayout.Width(180));
                ignorePlugins = EditorGUILayout.Toggle("IgnorePlugins", ignorePlugins, GUILayout.Width(180));
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Execute"))
            {
                Execute();
            }
        }

        private bool ignorePlugins = true;
        private bool ignorePackages = true;

        private async void Execute()
        {
            // 遍历所有纹理， 并生成 TextureInfo
            string[] textGuids = AssetDatabase.FindAssets("t:Texture", new [] { "Assets" });

            _textDic.Clear();

            for (int i = 0; i < textGuids.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(textGuids[i]);
                
                if (ignorePackages && IsInPackages(path)) 
                    continue;
                
                if (ignorePlugins && IsInPlugins(path))
                    continue;
                
                GetTextureInfo(path);
            }

            Debug.Log("TextureInfo Count：" + _textDic.Count);

            foreach (var info in _textDic)
            {
                var refList = await ReferenceSpectorHelper.Find(info.Value.assetPath);

                if (refList != null)
                {
                    foreach (var referenceInfo in refList)
                    {
                        info.Value.referencePaths.Add(referenceInfo.Path);
                    }
                }
            }

            // 将纹理信息引用次数降序排序
            TextureInfo[] textureInfos = _textDic
                .Values
                .OrderBy(value => value.referenceCount, Comparer<int>.Create((x, y) => y.CompareTo(x)))
                .ToArray();

            Dictionary<string, TextureInfo> _textInfoDic = DictionaryPool<string, TextureInfo>.Get();

            // 合并重复的资源信息
            for (int i = 0; i < textureInfos.Length; ++i)
            {
                var textInfo = textureInfos[i];
                if (_textInfoDic.TryGetValue(textInfo.base64, out var info))
                {
                    info.duplicateList.Add(textInfo);
                }
                else
                {
                    _textInfoDic.Add(textInfo.base64, textInfo);
                }
            }
            
            Debug.Log($"Total texture count: {_textInfoDic.Count}");
            

            foreach (var info in _textInfoDic)
            {
                var textureInfo = info.Value;
                Debug.Log($"name: {textureInfo.name}, refCount:{textureInfo.referenceCount} path:{textureInfo.assetPath} duplicateCount：{textureInfo.duplicateCount}");
            }

            DictionaryPool<string, TextureInfo>.Release(_textInfoDic);
        }


        [MenuItem("Assets/Test")]
        public static void Test()
        {
            GetSpriteAtlasDiskSize("Assets/Art/Atlas/SpriteAtlas.spriteatlas");
            if (Selection.activeObject == null) return;
        }

        private static Dictionary<string, TextureInfo> _textDic = new();
        
        /// <summary>
        /// 获取纹理详情
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private static TextureInfo GetTextureInfo(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return null;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            
            if (_textDic.ContainsKey(guid))
                return _textDic[guid];
            
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
            string absolutePath = GetAssetAbsolutePath(assetPath);
            var base64Str = ImageToBase64(absolutePath);

            TextureInfo textureInfo = new TextureInfo()
            {
                name = Path.GetFileName(assetPath),
                memorySize = GetTextureMemorySize(texture),
                runtimeMemorySize = GetTextureRuntimeMemorySize(texture),
                assetPath = assetPath,
                base64 = base64Str,
                guid = guid,
            };
            
            _textDic.Add(guid, textureInfo);

            return textureInfo;
        }

    }

    public class TextureInfo
    {
        /// <summary>
        /// 图片名字
        /// </summary>
        public string name;
        
        /// <summary>
        /// 被引用的路径
        /// </summary>
        public List<string> referencePaths = new List<string>();

        /// <summary>
        /// 被多少个资源所引用
        /// </summary>
        public int referenceCount => referencePaths.Count;

        /// <summary>
        /// Assets 路径
        /// </summary>
        public string assetPath;

        /// <summary>
        /// 硬盘文件大小
        /// </summary>
        public long memorySize;
        
        /// <summary>
        /// 内存占用大小
        /// </summary>
        public long runtimeMemorySize;

        /// <summary>
        /// 重复图片
        /// </summary>
        public List<TextureInfo> duplicateList = new List<TextureInfo>();

        /// <summary>
        /// 重复次数
        /// </summary>
        public int duplicateCount => duplicateList.Count;

        public string base64;

        public string guid;

        public string formatMemoryBytes => EditorUtility.FormatBytes(memorySize);
        public string formatRuntimeMemoryBytes => EditorUtility.FormatBytes(runtimeMemorySize);
    }
}