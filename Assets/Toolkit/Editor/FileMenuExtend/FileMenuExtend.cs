using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Editor.FileMenuExtend
{
    public class FileMenuExtend
    {
        public static class FileMenuExtendUtils
        {
            [MenuItem("File/Open Editor Folder", false, 0)]
            static void OpenEditorFolder()
            {
                string editorPath = Path.GetDirectoryName(EditorApplication.applicationContentsPath);
                Application.OpenURL("file://" + editorPath);
            }

            [MenuItem("File/Open Project Folder", false, 0)]
            static void OpenProjectFolder()
            {
                string projectPath = Path.GetDirectoryName(Application.dataPath);
                Application.OpenURL("file://" + projectPath);
            }
        }
    }
}