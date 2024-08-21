using System;
using System.Reflection;
using UnityEngine.EventSystems;

namespace Utils
{
    /// <summary>
    /// 通过反射代理全局点击事件
    /// </summary>
    public class PointerClickGlobalProxy : SingletonBase<PointerClickGlobalProxy>
    {
        public event Action<PointerEventData> OnClickListeners;

        private PointerClickGlobalProxy()
        {
            Type t = typeof(ExecuteEvents);
            FieldInfo fieldInfo = t.GetField("s_PointerClickHandler", BindingFlags.Static | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, new ExecuteEvents.EventFunction<IPointerClickHandler>(OnPointerClickProxy));
            }
        }

        private void OnPointerClickProxy(IPointerClickHandler handler, BaseEventData eventData)
        {
            PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            handler.OnPointerClick(pointerEventData);
            OnClickListeners?.Invoke(pointerEventData);
        }

        public void RemoveAllListeners()
        {
            OnClickListeners = null;
        }
    }
}
