using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EditorExtension
{
    public static class MenuItemExtension
    {
        /// <summary>
        /// 创建默认取消勾选 Raycast 的 Image
        /// </summary>
        [MenuItem("GameObject/UI/ImageWithoutRaycast")]
        static void CreateImageWithoutRaycast()
        {
            if (Selection.activeTransform != null && 
                Selection.activeTransform.GetComponentInParent<Canvas>() != null)
            {
                Image image = new GameObject("Image").AddComponent<Image>();
                image.raycastTarget = false;
                image.transform.SetParent(Selection.activeTransform, false);
                
                // 设置当前Image为选中状态
                Selection.activeTransform = image.transform;
            }
        }
    }
}