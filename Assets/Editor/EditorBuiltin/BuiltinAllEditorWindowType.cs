using UnityEditor;
using UnityEngine;

namespace EditorExtension.EditorBuiltin
{
    public static class BuiltinAllEditorWindowType
    {
        [MenuItem("Tools/GetAllEditorWindowType")]
        static void GetAllEditorWindow()
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                Debug.Log(window.GetType().ToString());
            }
        }
    }
}
