using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.EditorWindows.Tools
{
    public static class MigrationResourcesToAssetBundle
    {
        [MenuItem("Tools/MigrationResourcesToAssetBundle")]
        static void Execute()
        {
            string outPath = Path.Combine(Application.dataPath, "streamingAssets");

            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            
            // TODO: 设置资源以及Bundle名
            builds.Add(new AssetBundleBuild()
            {
                assetBundleName = "Cube.unity3d",
                assetNames = new string[]
                {
                    "Assets/Resources/Cube.prefab",
                    "Assets/Resources/Cube 1.prefab"
                }
            });

            // 构建AssetBundle
            BuildPipeline.BuildAssetBundles(outPath, builds.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle,
                BuildTarget.StandaloneWindows);
            
            // 生成描述文件
            BundleList bundleList = ScriptableObject.CreateInstance<BundleList>();
            foreach (AssetBundleBuild item in builds)
            {
                foreach (string assetName in item.assetNames)
                {
                    bundleList.bundleDatas.Add(new BundleList.BundleData() { resPath = assetName, bundlePath = item.assetBundleName });
                }
            }
            
            AssetDatabase.CreateAsset(bundleList, "Assets/Resources/bundleList.asset");
        }
    }

    [Serializable]
    public class BundleList : ScriptableObject
    {
        public List<BundleData> bundleDatas = new List<BundleData>();

        [Serializable]
        public class BundleData
        {
            /// <summary>
            /// Resource 路径
            /// </summary>
            public string resPath = string.Empty;
            /// <summary>
            /// AssetBundle 路径
            /// </summary>
            public string bundlePath = string.Empty;
        }
    }
}