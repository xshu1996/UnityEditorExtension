using System;
using System.Collections.Generic;
using System.IO;
using EditorExtension.Styles;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class MigrationLuaToTxtEditor : EditorWindow
{
    [MenuItem("Tools/MigrationLuaToTxt")]
    static void OpenWindow()
    {
        MigrationLuaToTxtEditor window = (MigrationLuaToTxtEditor)EditorWindow.GetWindow(typeof(MigrationLuaToTxtEditor));
        window.minSize = new Vector2(800, 500);
        window.Show();
    }

    private void Awake()
    {
        _isCleanOldFile = EditorPrefs.GetInt(GetPrefsKey("IsCleanOldFile"), 0) > 0;
        _srcPath = EditorPrefs.GetString(GetPrefsKey("SrcPath"));
        _dstPath = EditorPrefs.GetString(GetPrefsKey("DstPath"));
        _bundleName = EditorPrefs.GetString(GetPrefsKey("BundleName"));
        
        UpdateAssetList();
    }

    private string GetPrefsKey(string str)
    {
        return nameof(MigrationLuaToTxtEditor) + "_" + str;
    }

    private bool _isCleanOldFile;
    private bool isCleanOldFile
    {
        get => _isCleanOldFile;
        set
        {
            if (value != _isCleanOldFile)
            {
                _isCleanOldFile = value;
                EditorPrefs.SetInt(GetPrefsKey("IsCleanOldFile"), Convert.ToInt32(isCleanOldFile));
            }
        }
    }

    private string _srcPath;

    private string srcPath
    {
        get => _srcPath;
        set
        {
            if (value != _srcPath)
            {
                _srcPath = value;
                EditorPrefs.SetString(GetPrefsKey("SrcPath"), srcPath);
                UpdateAssetList();
            }
        }
    }

    private void UpdateAssetList()
    {
        // 更新列表
        if (Directory.Exists(srcPath))
        {
            dstUrls = new List<string>(Directory.GetFiles(srcPath, "*.lua"));
            dstSelected = new List<string>(dstUrls);
        }
        else
        {
            dstUrls.Clear();
            dstSelected.Clear();
        }
    }

    private string _dstPath;

    private string dstPath
    {
        get => _dstPath;
        set
        {
            if (value != _dstPath)
            {
                _dstPath = value;
                EditorPrefs.SetString(GetPrefsKey("DstPath"), dstPath);
            }
        }
    }

    private string _bundleName;

    private string bundleName
    {
        get => _bundleName;
        set
        {
            if (value != _bundleName)
            {
                _bundleName = value;
                EditorPrefs.SetString(GetPrefsKey("BundleName"), _bundleName);
            }
        }
    }

    private List<string> dstUrls;
    private List<string> dstSelected;
    private readonly float blockSpace = 10f;
    private void OnGUI()
    {
        EditorGUILayout.Space(blockSpace);
        
        GUILayout.Label("Input Bundle Name");
        bundleName = EditorGUILayout.TextField("BundleName:", bundleName);
        
        EditorGUILayout.Space(blockSpace);
        GUILayout.Label("Select Lua Scripts Root Directory");
        GUILayout.BeginHorizontal();
        {
            srcPath = EditorGUILayout.TextField("SourcePath:", srcPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60f)))
            {
                srcPath = EditorUtility.OpenFolderPanel("Open Lua Scripts Root Directory", Application.dataPath, String.Empty);
            }
        }
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space(blockSpace);
        GUILayout.Label("Select Migration Destination Directory");
        GUILayout.BeginHorizontal();
        {
            dstPath = EditorGUILayout.TextField("DestinationPath:", dstPath);

            if (GUILayout.Button("Browse", GUILayout.Width(60f)))
            {
                dstPath = EditorUtility.OpenFolderPanel("Open Lua Scripts Migration Destination Directory", Application.dataPath, String.Empty);
            }
        }
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space(blockSpace * 2);
        // 渲染列表
        OnGUIFileList(dstUrls);

        EditorGUILayout.Space(blockSpace);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            isCleanOldFile = EditorGUILayout.Toggle("ClearOldTxtAssets", isCleanOldFile, GUILayout.Width(180));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(blockSpace);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute", GUILayout.Width(100), GUILayout.Height(20)))
            {
                Executor(srcPath, dstPath, bundleName);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(blockSpace * 3);
    }

    void Executor(string srcPath, string dstPath, string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName))
        {
            return;
        }
        
        if (!Directory.Exists(srcPath))
        {
            Debug.LogWarning($"{srcPath} isn't Exists");
            return;
        }

        if (string.IsNullOrEmpty(dstPath))
        {
            Debug.Log("Please selected output directory");
            return;
        }

        // 判断路径是否在项目内
        if (dstPath.IndexOf(Application.dataPath) == -1)
        {
            Debug.LogWarning("Please select the path within the project");
            return;
        }
        
        // 判断是否需要清理旧文件
        if (Directory.Exists(dstPath))
        {
            if (isCleanOldFile)
            {
                string[] oldFiles = Directory.GetFiles(dstPath, "*.txt");
                for (int i = 0; i < oldFiles.Length; ++i)
                {
                    File.Delete(oldFiles[i]);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(dstPath);
        }

        string[] newFileUrls = new string[dstSelected.Count];
        for (int i = 0; i < dstSelected.Count; ++i)
        {
            string url = dstSelected[i];
            string fileName = dstPath + url.Substring(url.LastIndexOf("/")) + ".txt";
            File.Copy(url, fileName);
            newFileUrls[i] = fileName;
        }
        
        AssetDatabase.Refresh();
        
        // 必须要在刷新后设置bundle name
        for (int i = 0; i < newFileUrls.Length; ++i)
        {
            string url = newFileUrls[i];
            // 传入相对Asset路径： Assets/.../
            AssetImporter importer = AssetImporter.GetAtPath(url.Substring(url.IndexOf("Assets")));
            if (importer != null)
                importer.assetBundleName = bundleName;
        }
        
        Debug.Log($"*********** Success Copy {newFileUrls.Length} Files At {DateTime.Now} ***********");
    }

    private const int MinListRow = 5;
    private Vector2 _scrollPosition;
    /// <summary>
    /// 渲染文件列表
    /// </summary>
    /// <param name="files"></param>
    void OnGUIFileList(List<string> files)
    {
        EditorGUILayout.BeginHorizontal(CustomGUIStyles.TableOddRowStyle);
        {
            EditorGUILayout.LabelField("Pick", GUILayout.Width(40), GUILayout.Height(20));
            EditorGUILayout.LabelField("No.", GUILayout.Width(40), GUILayout.Height(20));
            EditorGUILayout.LabelField("Path", GUILayout.ExpandWidth(true), GUILayout.Height(20));
            EditorGUILayout.LabelField("Asset");
        }
        EditorGUILayout.EndHorizontal();
        
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
        {
            int row = Math.Max(files.Count, MinListRow);

            const int rowHeight = 20;
            for (int i = 0; i < row; ++i)
            {
                GUIStyle lineStyle =
                    (i & 1) == 0 ? CustomGUIStyles.TableEvenRowStyle : CustomGUIStyles.TableOddRowStyle;

                EditorGUILayout.BeginHorizontal(lineStyle, GUILayout.Height(rowHeight));
                {
                    // 判断是否在视图内， 如果不在直接渲染空行
                    int posY = i * rowHeight;
                    bool isInView = (posY > _scrollPosition.y - 60) && posY < _scrollPosition.y + position.height;
                    if (isInView && files.Count > i)
                    {
                        string fileName = files[i];
                        
                        bool selected = dstSelected.Contains(fileName);
                        selected = EditorGUILayout.Toggle("", selected, GUILayout.Width(40));
                        if (selected)
                        {
                            if (!dstSelected.Contains(fileName))
                                dstSelected.Add(fileName);
                        }
                        else
                        {
                            dstSelected.Remove(fileName);
                        }
                        
                        EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(40),
                            GUILayout.Height(20));

                        string internalName = fileName.Substring(fileName.IndexOf("Assets"));
                        
                        // show internal path
                        EditorGUILayout.SelectableLabel(internalName, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                        
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(internalName);
                        if (obj != null)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.ObjectField(obj, typeof(Object), false);
                            EditorGUI.EndDisabledGroup();
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Missing Asset", CustomGUIStyles.TableLabelStyle);
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
