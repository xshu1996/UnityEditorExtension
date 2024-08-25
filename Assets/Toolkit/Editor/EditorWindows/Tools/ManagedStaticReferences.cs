using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Toolkit.EditorExtension.EditorWindows.Tools
{
    public class ManagedStaticReferences : Editor
    {
        /// <summary>
        /// 查找资源静态引用
        /// 避免被静态引用的资源无法被GC卸载掉
        /// </summary>
        /// <returns></returns>
        [MenuItem("Tools/Report/ResourceStaticReference")]
        static void StaticRef()
        {
            // 通过反射 Assembly-CSharp-firstpass 和 Assembly-CSharp 两个 DLL 文件，获取静态引用
            LoadAssembly("Assembly-CSharp-firstpass");
            LoadAssembly("Assembly-CSharp");
        }

        static void LoadAssembly(string name)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(name);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            finally
            {
                if (assembly != null)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        try
                        {
                            HashSet<string> assetPaths = new HashSet<string>();
                            FieldInfo[] listFieldInfo =
                                type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                            foreach (FieldInfo fieldInfo in listFieldInfo)
                            {
                                // 如果字段不是值类型
                                if (!fieldInfo.FieldType.IsValueType)
                                {
                                    SearchProperties(fieldInfo.GetValue(null), assetPaths);
                                }
                            }

                            if (assetPaths.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendFormat("{0}.cs\n", type.ToString());
                                foreach (string path in assetPaths)
                                {
                                    sb.AppendFormat("\t{0}\n", path);
                                }
                                
                                Debug.Log("-------------- Resources Static References Block --------------");
                                Debug.Log(sb.ToString());
                                Debug.Log("-------------- Resources Static References Block End --------------");
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                    }
                }
            }
        }

        static HashSet<string> SearchProperties(object obj, HashSet<string> assetPaths)
        {
            if (obj != null)
            {
                if (obj is UnityEngine.Object)
                {
                    UnityEngine.Object[] depen = EditorUtility.CollectDependencies(new UnityEngine.Object[]
                        { obj as UnityEngine.Object });

                    foreach (var o in depen)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(o);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            if (!assetPath.Contains(assetPath))
                            {
                                assetPaths.Add(assetPath);
                            }
                        }
                    }
                }
                else if (obj is IEnumerable)
                {
                    foreach (var child in obj as IEnumerable)
                    {
                        SearchProperties(child, assetPaths);
                    }
                }
                else if (obj is System.Object)
                {
                    if (!obj.GetType().IsValueType)
                    {
                        FieldInfo[] fieldInfos = obj.GetType().GetFields();
                        foreach (FieldInfo fieldInfo in fieldInfos)
                        {
                            object o = fieldInfo.GetValue(obj);
                            if (o != obj)
                            {
                                SearchProperties(o, assetPaths);
                            }
                        }
                    }
                }
            }

            return assetPaths;
        }
    }
}
