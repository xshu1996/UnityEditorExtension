using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Utils
{
    public static class UIUtils
    {
        /// <summary>
        /// 为UGUI节点添加指定类型的事件监听
        /// </summary>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <param name="listener"></param>
        public static void AddEventTriggerListener(GameObject target, EventTriggerType triggerType, UnityAction<BaseEventData> listener)
        {
            var eventTrigger = target.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = target.AddComponent<EventTrigger>();
            }

            var entry = eventTrigger.triggers.Find(v => v.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry
                {
                    eventID = triggerType
                };
                eventTrigger.triggers.Add(entry);
            }

            entry.callback.AddListener(listener);
        }

        /// <summary>
        /// 为UGUI节点移除指定类型的事件监听
        /// </summary>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <param name="listener"></param>
        public static void RemoveEventTriggerListener(GameObject target, EventTriggerType triggerType,
            UnityAction<BaseEventData> listener)
        {
            var eventTrigger = target.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                return;
            }

            var entry = eventTrigger.triggers.Find(v => v.eventID == triggerType);
            if (entry == null)
            {
                return;
            }

            entry.callback.RemoveListener(listener);
        }
    }
}