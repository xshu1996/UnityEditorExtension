using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolkit.Editor.ReferencesSpector
{
    public class ReferencesFinder: EditorWindow
    {
        [MenuItem("Assets/References Finder")]
        static void OpenWindow()
        {
             ReferencesFinder window = GetWindow<ReferencesFinder>();
             window.minSize = new Vector2(800, 500);
             window.titleContent = new GUIContent() { text = nameof(ReferencesFinder) };
             window.Show();
        }

        [MenuItem("Assets/Find References Info")]
        static void FindAssetReferencesInfo()
        {
            if (!Selection.activeObject) return;

            ReferenceSpectorHelper.Find(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        private void OnGUI()
        {
            
        }
    }
    
    public static class ReferenceSpectorHelper
    {

        /// <summary>
        /// 获取项目中所有符合后缀的文件路径
        /// </summary>
        /// <returns></returns>
        public static string[] GetProjectAllAssets()
        {
            return Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(FileFilterByExtension)
                .ToArray();
        }

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
            
            return Application.dataPath + path;
        }

        /// <summary>
        /// 通过后缀过滤掉非指定文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileFilterByExtension(string path)
        {
            return BuiltinExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        
        private static readonly string[] BuiltinExtensions = new[]
        {
            ".unity", ".prefab", ".asset", ".mat", 
            ".shadervariants", ".fontsettings", ".cubemap", 
            ".flare", ".scenetemplate", ".mask", ".overrideController",
            ".terrainlayer", ".guiskin"
        };

        public static void FindByGuid(string guid, Action<List<ReferenceInfo>> callback = null)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Find(path, callback);
        }

        /// <summary>
        /// 通过路径查找引用
        /// </summary>
        /// <param name="assetPath">资源路径, 以 Assets 开始</param>
        /// <param name="callback"></param>
        public static void Find(string assetPath, Action<List<ReferenceInfo>> callback = null)
        {

            if (string.IsNullOrEmpty(assetPath))
            {
                callback?.Invoke(null);
                return;
            }

            var refList = new List<ReferenceInfo>();
            string[] files = GetProjectAllAssets();

            int fileCount = files.Length;
            int indicate = 0;

            // 分帧遍历, 每帧找10个
            const int perFrameCount = 10;
            EditorApplication.update = () =>
            {
                int count = perFrameCount;
                if (indicate + perFrameCount > fileCount)
                {
                    count = fileCount - indicate;
                }
                
                for (int i = 0; i < count; ++i)
                {
                    string relativePath = GetAssetRelativePath(files[indicate]);
                    
                    indicate++;

                    if (assetPath.Equals(relativePath))
                    {
                        continue;
                    }

                    string title = $"Finding References In Files... ({indicate}/{fileCount})";
                    float progress = (float) indicate / fileCount;
                    bool canceled = EditorUtility.DisplayCancelableProgressBar(title, relativePath, progress);
                    if (canceled)
                    {
                        EditorApplication.update = null;
                        EditorUtility.ClearProgressBar();
                        callback?.Invoke(null);
                        return;
                    }

                    int refCount = CheckReference(relativePath, assetPath);
                    if (refCount > 0)
                    {
                        refList.Add(new ReferenceInfo(relativePath, AssetDatabase.AssetPathToGUID(relativePath), refCount));
                    }
                }

                if (indicate >= fileCount)
                {
                    EditorApplication.update = null;
                    EditorUtility.ClearProgressBar();
                    callback?.Invoke(refList);

                    for (int i = 0; i < refList.Count; ++i)
                    {
                        Debug.Log($"Reference Info {i + 1} : {refList[i]}");
                    }
                    
                    Debug.Log("Execute Find End");
                }
            };
        }

        public static void FindRegex(Object asset, out List<ReferenceInfo> refList)
        {
            refList = new List<ReferenceInfo>();
            // 生成正则表达式，例如：
            // - 内置资源：{fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}
            // - 项目资源：{fileID: 100100000, guid: 57d31b7d2a71b42858f8d031d9c6219b, type: 3}
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string assetGuid, out long assetFileId);
        }

        public static int CheckReference(string dstPath, string srcPath)
        {
            string[] dependencies = AssetDatabase.GetDependencies(dstPath);
            if (dependencies.Contains(srcPath))
            {
                return 1;
            }

            return 0;
        }
        
        public static int CheckReference(string dstPath, Regex regex)
        {
            string text = File.ReadAllText(GetAssetAbsolutePath(dstPath));
            MatchCollection collection = regex.Matches(text);
            return collection.Count;
        }
    }

    public readonly struct ReferenceInfo
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// 资源的 GUID
        /// </summary>
        public readonly string GUID;

        /// <summary>
        /// 资源被引用的次数
        /// </summary>
        public readonly int RefCount;

        public ReferenceInfo(string path, string guid, int refCount)
        {
            Path = path;
            GUID = guid;
            RefCount = refCount;
        }
        
        public override string ToString()
        {
            return $"path: {Path}, GUID: {GUID}, refCount: {RefCount}";
        }
    }
}
