using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Gizmos.GizmosHelper
{
    public class RaycastGizmos : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Raycast Items Finder/Enable")]
        static void EnableRaycastItemsFinder()
        {
            if (FindObjectOfType<RaycastGizmos>() == null)
            {
                var go = new GameObject("RaycastItemsFinder");
                go.AddComponent<RaycastGizmos>();
            }
        }
        
        [MenuItem("Tools/Raycast Items Finder/Enable", true)]
        static bool ShowEnableRaycastItemsFinder()
        {
            var target = FindObjectOfType<RaycastGizmos>();
            return target == null;
        }
        
        [MenuItem("Tools/Raycast Items Finder/Disable")]
        static void DisableRaycastItemsFinder()
        {
            var target = FindObjectOfType<RaycastGizmos>();
            if (target != null)
            {
                // 编辑器删除建议使用Undo去删除，使用以下语句无法删除干净，删除后仍会执行OnDrawGizmos
                // DestroyImmediate(target.gameObject);
                Undo.DestroyObjectImmediate(target.gameObject);
            }
        }
        
        [MenuItem("Tools/Raycast Items Finder/Disable", true)]
        static bool ShowDisableRaycastItemsFinder()
        {
            var target = FindObjectOfType<RaycastGizmos>();
            return target != null;
        }
        
        private static readonly Vector3[] corners = new Vector3[4];
        private void OnDrawGizmos()
        {
            foreach (var graphic in FindObjectsOfType<MaskableGraphic>())
            {
                if (graphic.raycastTarget)
                {
                    RectTransform rect = graphic.transform as RectTransform;
                    if (rect == null) continue;

                    rect.GetWorldCorners(corners);
                    UnityEngine.Gizmos.color = Color.red;
                    for (int i = 0; i < corners.Length; i++)
                    {
                        UnityEngine.Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
                    }
                }
            }
        }
#endif
    }
}
