using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.EditorExtension
{
    public static class ShortcutExtension
    {
        /// <summary>
        /// Inspector 窗口锁定，快捷键： Ctrl/Command + Alt/Option + L
        /// </summary>
        [MenuItem("Window/Shortcut/Toggle Inspector Lock %&l")]
        private static void InspectorLockShortcut()
        {
            // 获取Inspector窗口
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var inspectorWindow = EditorWindow.GetWindow(inspectorType);

            // 使用反射调用isLocked属性
            var isLockedProperty = inspectorType.GetProperty("isLocked");
            var isLocked = (bool)isLockedProperty.GetValue(inspectorWindow);
            isLockedProperty.SetValue(inspectorWindow, !isLocked);
        }

        /// <summary>
        /// 锁定与解锁选中的 GameObject 的 Inspector 面板，快捷键： Ctrl/Command + Shift + L
        /// </summary>
        [MenuItem("GameObject/Shortcut/LockGameObject %#l")]
        private static void GameObjectLockShortcut()
        {
            if (Selection.gameObjects != null)
            {
                foreach (var gameObject in Selection.gameObjects)
                {
                    if ((gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
                    {
                        gameObject.hideFlags &= ~HideFlags.NotEditable;
                    }
                    else
                    {
                        gameObject.hideFlags |= HideFlags.NotEditable;
                    }
                }
            }
        }

        /// <summary>
        /// 清空控制台打印，快捷键：Ctrl/Command + L
        /// </summary>
        [MenuItem("Tools/Shortcut/CleanConsole %L")]
        private static void CleanConsole()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            MethodInfo methodInfo = assembly.GetType("UnityEditor.LogEntries").GetMethod("Clear");
            methodInfo.Invoke(new object(), null);
        }

    }
}
