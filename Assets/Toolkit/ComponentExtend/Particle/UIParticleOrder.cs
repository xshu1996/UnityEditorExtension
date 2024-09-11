using UnityEngine;

namespace Toolkit.ComponentExtend.Particle
{
    [AddComponentMenu("UI/Custom/UIParticleOrder")]
    public class UIParticleOrder: MonoBehaviour
    {
        [SerializeField] 
        private int m_SortingOrder = 0;

        public int sortingOrder
        {
            get => m_SortingOrder;
            set
            {
                if (m_SortingOrder != value)
                {
                    m_SortingOrder = value;
                    Refresh();
                }
            }
        }

        private Canvas m_Canvas = null;

        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = gameObject.GetComponent<Canvas>();
                    if (m_Canvas == null)
                        m_Canvas = gameObject.AddComponent<Canvas>();
                    m_Canvas.hideFlags = canEditCanvas ? HideFlags.None : HideFlags.NotEditable;
                }

                return m_Canvas;
            }
        }

        private void Refresh()
        {
            canvas.hideFlags = canEditCanvas ? HideFlags.None : HideFlags.NotEditable;
            // 不随父对象的sorting属性，可以实现子对象在父对象的后面
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            foreach (var particle in GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                particle.sortingOrder = sortingOrder;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Refresh();
        }

        private void Reset()
        {
            Refresh();
        }

        public bool canEditCanvas = false;
#endif
    }
}