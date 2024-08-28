using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string ImageToBase64(string absolutePath)
        {
            FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            
            fs.Read(buffer, 0, (int)fs.Length);
            
            string base64String = Convert.ToBase64String(buffer);
            Debug.Log("获取当前图片base64为---" + base64String);
            return base64String;
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
            
        }


        [MenuItem("Assets/Test")]
        public static void Test()
        {
            if (Selection.activeObject != null)
            {
                GetTextureInfo(AssetDatabase.GetAssetPath(Selection.activeObject));
            }
        }
        
        private static Dictionary<string, TextureInfo> _textDic = new();
        private static void GetTextureInfo(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return;
            
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            string absolutePath = GetAssetAbsolutePath(assetPath);
            ImageToBase64(absolutePath);
        }

    }

    public struct TextureInfo
    {
        /// <summary>
        /// 引用次数
        /// </summary>
        public int referenceCount;

        /// <summary>
        /// Assets 路径
        /// </summary>
        public string assetPath;

        /// <summary>
        /// 文件大小
        /// </summary>
        public int fileSize;

        /// <summary>
        /// 重复图片
        /// </summary>
        public List<TextureInfo> duplicate;

        public string base64;
    }
}