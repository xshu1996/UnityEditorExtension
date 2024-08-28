using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Editor.EditorBuiltin
{
   public class BuiltinIconsWindow : EditorWindow
   {
      [MenuItem("Window/Builtin/BuiltinIcons")]
      static void OpenBuiltinIconWindow()
      {
         ((BuiltinIconsWindow)EditorWindow.GetWindow(typeof(BuiltinIconsWindow))).Show();
      }

      private Vector2 m_Scroll;
      private List<string> m_Icons;

      private void Awake()
      {
         m_Icons = new List<string>();

         Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();

         foreach (Texture2D tex in t)
         {
            // 屏蔽加载不到图片资源的报错
            Debug.unityLogger.logEnabled = false;
            GUIContent gc = EditorGUIUtility.IconContent(tex.name);
            Debug.unityLogger.logEnabled = true;
            if (gc != null && gc.image != null)
            {
               m_Icons.Add(tex.name);
            }
         }
         Debug.Log("Icon Count: " + m_Icons.Count);
      }

      private void OnGUI()
      {
         m_Scroll = GUILayout.BeginScrollView(m_Scroll);

         float width = 50f;
         int count = (int)(position.width / width);

         for (int i = 0; i < m_Icons.Count; i += count)
         {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < count; ++j)
            {
               int index = i + j;
               if (index < m_Icons.Count)
               {
                  if (GUILayout.Button(EditorGUIUtility.IconContent(m_Icons[index]), GUILayout.Width(width),
                         GUILayout.Height(30)))
                  {
                     Debug.Log("Texture2D Name: " + m_Icons[index]);
                  }
               }
            }
            GUILayout.EndHorizontal();
         }
         
         GUILayout.EndScrollView();
      }
   }
}
