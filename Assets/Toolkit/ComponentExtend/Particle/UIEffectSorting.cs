using System;
using UnityEngine;
using UnityEngine.UI;

public class UIEffectSorting : MonoBehaviour
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

    private Canvas m_parentCanvas
    {
        get
        {
            return GetComponentInParent<Canvas>(true);
        }
    }
    
    void OnEnable()
    {
        Refresh();
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        Refresh();
    }
#endif
    
    void Refresh()
    {
        int order = m_parentCanvas.sortingOrder + sortingOrder;
        foreach (var canvas in GetComponentsInChildren<Canvas>(true))
        {
            canvas.sortingOrder = order;
        }

        foreach (var render in GetComponentsInChildren<Renderer>(true))
        {
            render.sortingOrder = order;

            if (render.sharedMaterial != null && render.sharedMaterial.renderQueue < 3000)
            {
                render.sharedMaterial.renderQueue = 3000;
            }
        }
    }
}
