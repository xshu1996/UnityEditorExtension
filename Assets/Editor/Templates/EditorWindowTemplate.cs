using System;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Template
{
    public class EditorWindowTemplate : EditorWindow, IHasCustomMenu
    {

        private bool m_Selected = false;
        /// <summary>
        /// 自定义EditorWindow右上角下拉菜单
        /// </summary>
        /// <param name="menu"></param>
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // 添加置灰菜单选项
            menu.AddDisabledItem(new GUIContent("Disable Item"));
            // 添加可勾选的菜单
            menu.AddItem(new GUIContent("Test1"), m_Selected, () =>
            {
                m_Selected = !m_Selected;
                Debug.Log("默认勾选的菜单 Test1" + m_Selected);
            });
            // 添加无法勾选的菜单
            menu.AddItem(new GUIContent("Test2"), false, () =>
            {
                Debug.Log("默认勾选的菜单 Test2");
            });
            
            // 添加子路径
            menu.AddSeparator("Test/");
            menu.AddItem(new GUIContent("Test/Test3"), false, () =>
            {
                Debug.Log("Test3");
            });
        }
        
        [MenuItem("Window/Template/CustomWindow")]
        static void Init()
        {
            EditorWindowTemplate window = (EditorWindowTemplate)EditorWindow.GetWindow(typeof(EditorWindowTemplate));
            window.Show();
        }

        private Texture m_MyTexture = null;
        private float m_MyFloat = 0.5f;

        private void Awake()
        {
            Debug.Log("On Window Awake");

            m_MyTexture = AssetDatabase.LoadAssetAtPath<Texture>("Asset/unity.png");
        }

        private void OnGUI()
        {
            GUILayout.Label("Hello Template Window Display", EditorStyles.boldLabel);
            m_MyFloat = EditorGUILayout.Slider("MyFloat", m_MyFloat, -5, 5);
        }

        private void OnDestroy()
        {
            Debug.Log("On Window Destroy Invoke");
        }

        private void OnFocus()
        {
            Debug.Log("On Window Focus Invoke");
        }

        private void OnHierarchyChange()
        {
            Debug.Log("On Hierarchy Changed");
        }

        private void OnInspectorUpdate()
        {
            // Debug.Log("On Inspector Update Every Frame");
        }

        private void OnProjectChange()
        {
            Debug.Log("On Project View Changed");
        }

        private void OnSelectionChange()
        {
            Debug.Log("在 Hierarchy 或者 Project 视图中选择一个对象时调用");
        }

        private void Update()
        {
            // Debug.Log("Invoke On Every Frame");
        }
    }
}
