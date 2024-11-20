using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.ComponentExtend.Particle
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Custom/UIParticleMask")]
    public class UIParticleMask : Mask
    {
        /// <summary>
        /// minX, minY, maxX, maxY
        /// </summary>
        private Vector4 m_LastBound = new Vector4(-1, -1, -1, -1);
        private Vector4 m_Bound = new Vector4(0, 0, 0, 0);
        private Vector3[] m_Corners = new Vector3[4];

        private void GetWorldCorners()
        {
            if (!Mathf.Approximately(m_LastBound.x, m_Bound.x) ||
                !Mathf.Approximately(m_LastBound.y, m_Bound.y) ||
                !Mathf.Approximately(m_LastBound.z, m_Bound.z) ||
                !Mathf.Approximately(m_LastBound.w, m_Bound.w))
            {
                RectTransform rectTransform = transform as RectTransform;
                rectTransform.GetWorldCorners(m_Corners);

                m_LastBound = m_Bound;
                m_Bound = new Vector4(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x, m_Corners[2].y); 
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Refresh();
        }

        public void Refresh()
        {
            GetWorldCorners();
            if (Application.isPlaying)
            {
                foreach (var particleSystemRenderer in GetComponentsInChildren<ParticleSystemRenderer>(true))
                {
                    SetRenderer(particleSystemRenderer);
                }
            }
        }

        private void SetRenderer(Renderer renderer)
        {
            if (renderer.sharedMaterial != null)
            {
                // TODO: 配合资源管理器引用计数
                Shader shader = Resources.Load<Shader>("UIParticleMask/Alpha Blended Premultiply");
                
                Material mat = renderer.material;
                mat.shader = shader;
                mat.SetFloat("_MinX", m_Bound.x);
                mat.SetFloat("_MinY", m_Bound.y);
                mat.SetFloat("_MaxX", m_Bound.z);
                mat.SetFloat("_MaxY", m_Bound.w);
            }
        }
    }
}