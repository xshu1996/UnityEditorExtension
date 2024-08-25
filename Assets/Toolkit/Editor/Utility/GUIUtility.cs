using UnityEditor;
using UnityEngine;

namespace Toolkit.EditorExtension.Utility
{
    public static class GUIUtility
    {
        /// <summary>
        /// 创建一个单色纹理
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static Texture2D GenerateSingleColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[] { color });
            texture.Apply();
            return texture;
        }
        
        /// <summary>
        /// 判断资源是否为 Unity 内置资源
        /// </summary>
        /// <param name="asset">资源</param>
        /// <returns></returns>
        private static bool IsUnityBuiltinAsset(Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return path.Equals("Resources/unity_builtin_extra") || path.Equals("Library/unity default resources");
        }
    }
}