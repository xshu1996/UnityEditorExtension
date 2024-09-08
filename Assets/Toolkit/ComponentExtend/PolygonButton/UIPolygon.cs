#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.ComponentExtend.PolygonButton
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class UIPolygon : Image
    {
        private PolygonCollider2D m_Polygon = null;

        public PolygonCollider2D polygon
        {
            get
            {
                if (m_Polygon == null)
                    m_Polygon = GetComponent<PolygonCollider2D>();
                return m_Polygon;
            }
        }

        protected UIPolygon()
        {
            useLegacyMeshGeneration = true;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            // 需要用正交 Camera
            return polygon.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transform.position = Vector3.zero;
            float w = rectTransform.sizeDelta.x * 0.5f + 0.1f;
            float h = rectTransform.sizeDelta.y * 0.5f + 0.1f;

            polygon.points = new Vector2[]
            {
                new Vector2(-w, -h),
                new Vector2(w, -h),
                new Vector2(w, h),
                new Vector2(-w, h)
            };
        }
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(UIPolygon), true)]
    public class UIPolygonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            // do nothing
        }
    }
#endif
}