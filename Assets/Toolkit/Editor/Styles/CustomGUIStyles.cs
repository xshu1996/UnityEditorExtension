using UnityEditor;
using UnityEngine;

namespace Toolkit.EditorExtension.Styles
{
    public class CustomGUIStyles
    {
        /// <summary>
        /// 获取 Unity 内置 GUI 风格
        /// </summary>
        /// <param name="styleName">风格名称</param>
        /// <returns></returns>
        private static GUIStyle GetBuiltinGUIStyle(string styleName)
        {
            GUIStyle style = GUI.skin.FindStyle(styleName) ??
                             EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            
            if (style == null)
            {
                Debug.LogError((object)("Missing built-in guistyle " + styleName));
            }
            
            return style;
        }
        
        /// <summary>
        /// 表格条目单数行风格
        /// </summary>
        public static readonly GUIStyle TableOddRowStyle = new GUIStyle(GUIStyle.none)
        {
            normal = { background = Utility.GUIUtility.GenerateSingleColorTexture(new Color(0.5f, 0.5f, 0.5f, 0.2f)) },
        };

        /// <summary>
        /// 表格条目双数行风格
        /// </summary>
        public static readonly GUIStyle TableEvenRowStyle = new GUIStyle(GUIStyle.none)
        {
            normal = { background = Utility.GUIUtility.GenerateSingleColorTexture(new Color(0.5f, 0.5f, 0.5f, 0.1f)) },
        };
        
        /// <summary>
        /// 表格条目文本风格
        /// </summary>
        public static readonly GUIStyle TableLabelStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            stretchWidth = true,
            stretchHeight = true,
            padding = new RectOffset(5, 5, 1, 1),
            margin = new RectOffset(0, 0, 0, 0),
        };
    }
}