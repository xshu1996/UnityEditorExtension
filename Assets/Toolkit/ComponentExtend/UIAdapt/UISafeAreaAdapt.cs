using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.ComponentExtend.UIAdapt
{
    [AddComponentMenu("UI/Custom/UISafeAreaAdapt")]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class UISafeAreaAdapt : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();
            _rectTransform = GetComponent<RectTransform>();
            Adapt();
        }

#if UNITY_EDITOR
        private void Update()
        {
            Adapt();
        }
#endif

        protected void Adapt()
        {
            if (_canvasScaler == null)
            {
                return;
            }

            Rect safeArea = Screen.safeArea;
            Vector2 designResolution = _canvasScaler.referenceResolution;
            float matchWidthOrHeight = _canvasScaler.matchWidthOrHeight;

            int width = (int)(designResolution.x * (1 - matchWidthOrHeight) +
                              designResolution.y * Screen.width / Screen.height * matchWidthOrHeight);
            int height = (int)(designResolution.y * matchWidthOrHeight - designResolution.x *
                Screen.height / Screen.width * (matchWidthOrHeight - 1));

            float ratio = designResolution.y * matchWidthOrHeight / Screen.height -
                          designResolution.x * (matchWidthOrHeight - 1) / Screen.width;
            
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;

            _rectTransform.offsetMin = new Vector2(safeArea.x * ratio, safeArea.y * ratio);
            _rectTransform.offsetMax = new Vector2((safeArea.x + safeArea.width) * ratio - width,
                (safeArea.y + safeArea.height) * ratio - height);
        }
    }
}