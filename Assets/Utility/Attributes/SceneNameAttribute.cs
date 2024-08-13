using System;
using UnityEditor;
using UnityEngine;

namespace Utility.Attributes
{
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneName : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(SceneName))]
    public class SceneNameAttribute : PropertyDrawer
    {
        static GUIContent[] _scenes;
        GUIContent[] GetSceneNames()
        {
            GUIContent[] g = new GUIContent[EditorBuildSettings.scenes.Length];
            
            for (int i = 0; i < g.Length; ++i)
            {
                string[] splitResult = EditorBuildSettings.scenes[i].path.Split('/');
                
                string nameWithSuffix = splitResult[^1];
                g[i] = new GUIContent(nameWithSuffix.Substring(0, nameWithSuffix.Length - ".unity".Length));
            }

            return g;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _scenes = GetSceneNames();

            string cntString = property.stringValue;
            
            int selected = 0;
            for (int i = 1; i < _scenes.Length; ++i)
            {
                if (_scenes[i].text.Equals(cntString))
                {
                    selected = i;
                    break;
                }
            }
            
            selected = EditorGUI.Popup(position, label, selected, _scenes);
            
            string propertyVal = selected < _scenes.Length ? _scenes[selected].text : string.Empty;
            bool isChanged = propertyVal != property.stringValue;
            property.stringValue = propertyVal;
            
            if (GUI.changed && isChanged)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
#endif
}